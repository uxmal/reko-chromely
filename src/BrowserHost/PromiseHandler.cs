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
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class PromiseHandler : CefV8Handler
    {
        private readonly Action<PromiseTask> promiseBody;

        public PromiseHandler(Action<PromiseTask> promiseBody)
        {
            this.promiseBody = promiseBody;
        }

        /// <summary>
        /// Promise invocation
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

            var promiseTask = new PromiseTask(ctx, promiseBody, resolveCb, rejectCb);
            CefTaskRunner.GetForCurrentThread().PostTask(promiseTask);

            returnValue = CefV8Value.CreateUndefined();
            exception = null!;
            return true;
        }
    }
}
