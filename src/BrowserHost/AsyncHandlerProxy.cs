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
    public class AsyncHandlerProxy : CefV8Handler
    {
        private readonly Func<CefV8Value[], CefV8Value> func;

        public AsyncHandlerProxy(Func<CefV8Value[], CefV8Value> func)
        {
            this.func = func;
        }

        // Expects a callback in arg 0. //$TODO: change all this to a promise later.
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            /**
             * Create an asynchronous task on the render thread.
             * the task is given the current execution context
             * so that the task can call-back into JS code when it wants to
             **/
            var task = new AsyncTask(CefV8Context.GetCurrentContext(), func, arguments, arguments[0]);
            CefTaskRunner.GetForCurrentThread().PostTask(task);

            //$TODO: return a promise.
            returnValue = CefV8Value.CreateNull();
            exception = null!;
            return true;
        }

        private class AsyncTask : CefTask
        {
            private readonly CefV8Value callback;
            private readonly CefV8Context ctx;
            private readonly CefV8Value[] arguments;
            private readonly Func<CefV8Value[], CefV8Value> func;

            public AsyncTask(CefV8Context ctx, Func<CefV8Value[], CefV8Value> func, CefV8Value[] arguments, CefV8Value callback)
            {
                this.ctx = ctx;
                this.func = func;
                this.arguments = arguments;
                this.callback = callback;
            }

            protected override void Execute()
            {
                var result = func(arguments);
                //$TODO: resolve the promise
                ctx.Acquire(() => {
                    callback.ExecuteFunction(null, new CefV8Value[] { result });
                });
            }
        }
    }
}
