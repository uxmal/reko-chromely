using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class ArrayBufferHandler : CefV8ArrayBufferReleaseCallback
    {
        private GCHandle gch;

        public ArrayBufferHandler(GCHandle gch)
        {
            this.gch = gch;
        }

        protected override void ReleaseBuffer(IntPtr buffer)
        {
            Console.WriteLine("FREEEEE THE THING");
            gch.Free();
        }
    }

    public class PromiseTask : CefTask
    {
        private readonly CefV8Context ctx;

        public readonly CefV8Value[] Arguments;
        private readonly CefV8Value resolveCb;
        private readonly CefV8Value rejectCb;
        private CefV8Value toRun;
        private object result;

        public PromiseTask(
            CefV8Context ctx,
            CefV8Value[] arguments,
            CefV8Value resolveCb, CefV8Value rejectCb)
        {
            this.ctx = ctx;
            this.Arguments = arguments;
            this.resolveCb = resolveCb;
            this.rejectCb = rejectCb;

            this.toRun = CefV8Value.CreateUndefined();
            this.result = CefV8Value.CreateUndefined();
        }

        /// <summary>
        /// Defer the invocation of the JS Resolve method by posting a task onto the task 
        /// runner's queue.
        /// </summary>
        /// <param name="result">.NET value to be passed back to JS. It will be marshaled when the 
        /// <see cref="Execute"/> method is called.</param>
        public void Resolve(object result)
        {
            // Post a task on the ctx's taskrunner. You're allowed to do this from any thread.
            toRun = resolveCb;
            this.result = result;
            ctx.GetTaskRunner().PostTask(this);
        }

        /// <summary>
        /// Defer the invocation of the JS Resolve method by posting a task onto the task 
        /// runner's queue.
        /// </summary>
        /// <param name="reason">.NET value to be passed back to JS. It will be marshaled when the 
        /// <see cref="Execute"/> method is called.</param>   
        public void Reject(object reason)
        {
            // Post a task on the ctx's taskrunner. You're allowed to do this from any thread.
            toRun = this.rejectCb;
            this.result = reason;
            ctx.GetTaskRunner().PostTask(this);
        }

        protected override void Execute()
        {
            // We just got called from the JS runtime task runner. We are in a JS context,
            // so it's safeto create JS values.
            ctx.Acquire(() =>
            {
                var result = ConvertToJsValue(this.result);
                // Now we can call the resolve JS function with our converted value.
                this.toRun.ExecuteFunction(null, new CefV8Value[] { result });
            });
        }

        private static CefV8Value CreateArrayBuffer(byte[] arr)
        {
            var gch = GCHandle.Alloc(arr, GCHandleType.Pinned);
            var dataptr = gch.AddrOfPinnedObject();

            var cb = new ArrayBufferHandler(gch);
            return CefV8Value.CreateArrayBuffer(dataptr, (ulong)arr.Length, cb);
        }

        private static CefV8Value CreateArray(byte[] arr)
        {
            var val = CefV8Value.CreateArray(arr.Length);

            for(int i=0; i<arr.Length; i++)
            {
                val.SetValue(i, CefV8Value.CreateUInt(arr[i]));
            }
            return val;
        }

        private CefV8Value ConvertToJsValue(object result)
        {
            return result switch {
                string s => CefV8Value.CreateString(s),
                byte[] arr => CreateArray(arr),
                _ => throw new NotImplementedException()
            };
        }
    }
}
