using System;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public interface IPromiseHandlerFactory
    {
        CefV8Handler CreateHandler(Action<PromiseTask> promiseBody, CefV8Value[] arguments);
    }
}