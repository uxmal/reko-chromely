using Chromely;
using Chromely.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Host;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
    public class RekoBrowserHost : Window
    {
        public RekoBrowserHost(IChromelyNativeHost nativeHost, IChromelyConfiguration config, ChromelyHandlersResolver handlersResolver)
            : base(nativeHost, config, handlersResolver)
        {
        }

        protected override CefClient CreateClient()
        {
            return new RekoClient(_browserMessageRouter, _handlersResolver);
        }

        public override void Create(IntPtr hostHandle, IntPtr winXID)
        {
            CefRuntime.RegisterSchemeHandlerFactory("reko", "", new RekoSchemeHandlerFactory());
            base.Create(hostHandle, winXID);
        }
    }
}
