using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class PendingPromisesHandlerFactory : IPromiseHandlerFactory
    {
        public CefV8Handler CreateHandler(Action<PromiseTask> promiseBody, CefV8Value[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
