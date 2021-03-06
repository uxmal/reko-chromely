﻿#region
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

using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    /// <summary>
    /// This class implements an Execute method that is used to start the C# worker
    /// method. The method is run "asynchronously" somehow using the <see cref="DoAsyncWork(CefV8Context, PromiseTask)"/> method,
    /// and when it is completed or fails, posts a <see cref="PromiseTask"/> which fulfills the JS promise.
    /// </summary>
    public abstract class PromiseFulfiller : CefV8Handler
    {
        /// <summary>
        /// Start executing the promise worker on a thread pool thread.
        /// </summary>
        /// <param name="arguments">The two callback provided by the JS side, one for resolving
        /// and one for rejecting the JS Promise.
        /// </param>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var resolveCb = arguments[0];
            var rejectCb = arguments[1];

            var ctx = CefV8Context.GetCurrentContext();
            var promiseTask = new PromiseTask(ctx, resolveCb, rejectCb);

            DoAsyncWork(ctx, promiseTask);

            // A fulfiller function doesn't return anything -- it's equivalent to
            // a 'void' C# function, so return 'undefined' to JS side.
            returnValue = CefV8Value.CreateUndefined();
            exception = null!;
            return true;
        }


        protected abstract void DoAsyncWork(CefV8Context ctx, PromiseTask promiseTask);
    }
}
