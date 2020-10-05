using System;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public interface IPromiseHandlerFactory
    {
        CefV8Handler CreateHandler(Delegate promiseBody, object?[] arguments);
    }
}