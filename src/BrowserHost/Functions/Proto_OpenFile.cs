using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost.Functions
{
    class Proto_OpenFileHandlerFactory : IPromiseHandlerFactory
    {
        private readonly PendingPromisesRepository pendingPromises;

        public Proto_OpenFileHandlerFactory(PendingPromisesRepository pendingPromises)
        {
            this.pendingPromises = pendingPromises;
        }

        public CefV8Handler CreateHandler(Func<object?[], object?> promiseBody, object?[] arguments)
        {
            return new Proto_OpenFile(pendingPromises, promiseBody, arguments);
        }
    }

    public class Proto_OpenFile : PromiseHandler
    {
        private readonly PendingPromisesRepository pendingPromises;

        public Proto_OpenFile(
            PendingPromisesRepository pendingPromises,
            Func<object?[], object?> promiseBody,
            object?[] arguments)
            : base(promiseBody, arguments)
        {
            this.pendingPromises = pendingPromises;
        }

        protected override void OnExecuteSync(PromiseTask promise)
        {
            int promiseId = pendingPromises.AddPromise(promise);

            var ctx = CefV8Context.GetCurrentContext();
            var browser = ctx.GetBrowser();
            var frame = browser.GetMainFrame();
            
            var msg = CefProcessMessage.Create("openFileRequest");
            msg.Arguments.SetInt(0, (int)promiseId);

            frame.SendProcessMessage(CefProcessId.Browser, msg);
        }

        public static object? ExecuteAsync(object? [] arguments)
        {
            return null;
        }
    }
}
