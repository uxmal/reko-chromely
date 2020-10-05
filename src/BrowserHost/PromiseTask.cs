#region
// Copyright 2020 the Reko contributors.

// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the
// 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
#endregion

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
            CefV8Value resolveCb, CefV8Value rejectCb)
        {
            this.ctx = ctx;
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
                // so it's safe to create JS values.
                var result = ValueConverter.ConvertToJsValue(this.result);
                // Now we can call the resolve JS function with our converted value.
                this.toRun.ExecuteFunction(null, new CefV8Value[] { result });
            });
        }
    }
}
