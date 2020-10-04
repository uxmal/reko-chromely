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
using System.Security.Authentication.ExtendedProtection;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class PromiseHandler : CefV8Handler
    {
        private readonly Action<PromiseTask> promiseBody;
        private readonly CefV8Value[] callerArguments;

        public PromiseHandler(Action<PromiseTask> promiseBody, CefV8Value[] arguments)
        {
            this.promiseBody = promiseBody;
            this.callerArguments = arguments;
        }

        protected virtual void OnExecuteSync(PromiseTask promise)
        {
        }

        /// <summary>
        /// Start executing the promise.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <param name="arguments"></param>
        /// <param name="returnValue"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var resolveCb = arguments[0];
            var rejectCb = arguments[1];

            var ctx = CefV8Context.GetCurrentContext();
            var promiseTask = new PromiseTask(ctx, callerArguments, resolveCb, rejectCb);

            OnExecuteSync(promiseTask);

            Task.Run(() =>
            {
                // Start running C# code in its own thread, No JS calls are allowed at this point.
                promiseBody(promiseTask);
            });

            returnValue = CefV8Value.CreateUndefined();
            exception = null!;
            return true;
        }
    }
}
