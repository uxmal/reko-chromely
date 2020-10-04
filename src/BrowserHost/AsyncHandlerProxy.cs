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
    public class DefaultPromiseHandlerFactory : IPromiseHandlerFactory
    {
        public CefV8Handler CreateHandler(Action<PromiseTask> promiseBody, CefV8Value[] arguments)
        {
            return new PromiseHandler(promiseBody, arguments);
        }
    }

    public class AsyncHandlerProxy : CefV8Handler
    {
        private readonly Action<PromiseTask> func;
        private readonly CefPromiseFactory promiseFactory;
        private readonly IPromiseHandlerFactory handlerFactory;

        public AsyncHandlerProxy(Action<PromiseTask> func, CefPromiseFactory promiseFactory, IPromiseHandlerFactory handlerFactory = null)
        {
            this.func = func;
            this.promiseFactory = promiseFactory;
            if (handlerFactory != null)
            {
                this.handlerFactory = handlerFactory;
            } else
            {
                this.handlerFactory = new DefaultPromiseHandlerFactory();
            }
        }

        /// <summary>
        /// Execute the delegated code asynchronously.
        /// </summary>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            // create the promise body
            var promiseBody = CefV8Value.CreateFunction("(anonymous)", handlerFactory.CreateHandler(func, arguments));

            // create the promise object
            returnValue = promiseFactory.CreatePromise(promiseBody);

            exception = null!;
            return true;
        }
    }
}
