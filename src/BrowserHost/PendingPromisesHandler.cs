using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class PendingPromisesHandlerFactory : IPromiseHandlerFactory
    {
        public CefV8Handler CreateHandler(Delegate promiseBody, object?[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}
