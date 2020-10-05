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
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class AsyncHandlerProxy : CefV8Handler
    {
        private readonly Delegate func;
        private readonly CefPromiseFactory promiseFactory;

        public AsyncHandlerProxy(Delegate func, CefPromiseFactory promiseFactory)
        {
            this.func = func;
            this.promiseFactory = promiseFactory;
        }

        /// <summary>
        /// Execute the delegated code asynchronously.
        /// </summary>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            // Create the promise fulfiller.
            var oArgs = ValueConverter.ConvertFromJsValues(arguments);
            var fulfiller = CefV8Value.CreateFunction("(anonymous)", new ThreadPoolFulfiller(func, oArgs));

            // create the promise object
            returnValue = promiseFactory.CreatePromise(fulfiller);

            exception = null!;
            return true;
        }

        /// <summary>
        /// This class fulfills a JS promise by executing its workload on a thread pool task. Once the task is completed,
        /// it fulfills the promise via the supplied <see cref="PromiseTask"/>.
        /// </summary>
        public class ThreadPoolFulfiller : PromiseFulfiller
        {
            private readonly Delegate promiseWorker;
            private readonly object?[] callerArguments; // Arguments passed to the function that made the JS promise.

            public ThreadPoolFulfiller(Delegate promiseWorker, object?[] arguments)
            {
                this.promiseWorker = promiseWorker;
                this.callerArguments = arguments;
            }

            protected override void DoAsyncWork(CefV8Context ctx, PromiseTask promiseTask)
            {
                // Start running C# code in its own thread, no JS calls are allowed at this point.
                Task.Run(() =>
                {
                    try
                    {
                        var result = promiseWorker.Method.Invoke(promiseWorker.Target, callerArguments);
                        promiseTask.Resolve(result);
                    }
                    catch (Exception ex)
                    {
                        promiseTask.Reject(ex);
                    }
                });
            }
        }
    }
}
