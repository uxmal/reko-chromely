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
    /// <summary>
    /// Because the Open File dialog is broken, we have to hack around its limitations. The JS caller invokes:
    /// filename = await reko.OpenFile()
    /// This handler is responsible for creating a JS promise, which when resolved, starts a chain of RFC calls
    /// until finally a filename is returned by calling the resolve function of the JS promise.
    /// </summary>
    public class OpenFileHandler : CefV8Handler
    {
        private readonly CefPromiseFactory promiseFactory;
        private readonly PendingPromisesRepository pendingPromises;

        public OpenFileHandler(CefPromiseFactory promiseFactory, PendingPromisesRepository pendingPromises)
        {
            this.promiseFactory = promiseFactory;
            this.pendingPromises = pendingPromises;
        }
        
        /// <summary>
        /// This method returns a promise. The fulfiller function of that promise starts off the RFC chain
        /// to display an Open File dialog and return the value.
        /// </summary>
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var fulfiller = CefV8Value.CreateFunction("(anonymous)", new PendingPromiseFulfiller(pendingPromises));

            returnValue = promiseFactory.CreatePromise(fulfiller);
            exception = null!;
            return true;
        }

        /// <summary>
        /// This fulfilling function is called from JS with a (resolve, reject) pair, but we have to do OpenFile RPC first.
        /// </summary>
        private class PendingPromiseFulfiller : PromiseFulfiller
        {
            private readonly PendingPromisesRepository pendingPromises;

            public PendingPromiseFulfiller(PendingPromisesRepository pendingPromises)
            {
                this.pendingPromises = pendingPromises;
            }

            protected override void DoAsyncWork(CefV8Context ctx, PromiseTask promiseTask)
            {
                int promiseId = pendingPromises.AddPromise(promiseTask);

                var browser = ctx.GetBrowser();
                var frame = browser.GetMainFrame();
                var msg = CefProcessMessage.Create("openFileRequest");
                msg.Arguments.SetInt(0, (int)promiseId);

                // This kicks off the RFC chain.
                frame.SendProcessMessage(CefProcessId.Browser, msg);
            }
        }
    }
}
