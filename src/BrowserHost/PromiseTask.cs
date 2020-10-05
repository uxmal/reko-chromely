using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class PromiseTask : CefTask
    {
        private readonly CefV8Context ctx;

        private readonly CefV8Value resolveCb;
        private readonly CefV8Value rejectCb;
        private CefV8Value toRun;
        private object? result;

        public PromiseTask(
            CefV8Context ctx,
            object?[] arguments,
            CefV8Value resolveCb, CefV8Value rejectCb)
        {
            this.ctx = ctx;
            this.Arguments = arguments;
            this.resolveCb = resolveCb;
            this.rejectCb = rejectCb;

            this.toRun = CefV8Value.CreateUndefined();
            this.result = CefV8Value.CreateUndefined();
        }

        public object?[] Arguments { get; }

        /// <summary>
        /// Defer the invocation of the JS Resolve method by posting a task onto the task 
        /// runner's queue.
        /// </summary>
        /// <param name="result">.NET value to be passed back to JS. It will be marshaled when the 
        /// <see cref="Execute"/> method is called.</param>
        public void Resolve(object? result)
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
            ctx.Acquire(() =>
            {
                // We just got called from the JS runtime task runner. We are in a JS context,
                // so it's safeto create JS values.
                var result = ValueConverter.ConvertToJsValue(this.result);
                // Now we can call the resolve JS function with our converted value.
                this.toRun.ExecuteFunction(null, new CefV8Value[] { result });
            });
        }
    }
}
