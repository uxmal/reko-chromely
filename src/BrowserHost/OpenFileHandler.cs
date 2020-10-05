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
        /// This handler is called from JS with a (resolve, reject) pair, but we have to do OpenFile RPC first.
        /// </summary>
        private class PendingPromiseFulfiller : CefV8Handler
        {
            private readonly PendingPromisesRepository pendingPromises;

            public PendingPromiseFulfiller(PendingPromisesRepository pendingPromises)
            {
                this.pendingPromises = pendingPromises;
            }

            protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
            {
                var ctx = CefV8Context.GetCurrentContext();
                var resolveCb = arguments[0];
                var rejectCb = arguments[1];
                var promiseTask = new PromiseTask(ctx, resolveCb, rejectCb);

                int promiseId = pendingPromises.AddPromise(promiseTask);

                var browser = ctx.GetBrowser();
                var frame = browser.GetMainFrame();
                var msg = CefProcessMessage.Create("openFileRequest");
                msg.Arguments.SetInt(0, (int)promiseId);

                // This kicks off the RFC chain.
                frame.SendProcessMessage(CefProcessId.Browser, msg);

                returnValue = CefV8Value.CreateUndefined();
                exception = null!;
                return true;
            }
        }
    }
}
