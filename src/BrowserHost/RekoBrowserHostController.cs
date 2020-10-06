using Chromely;
using Chromely.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Host;
using Chromely.Core.Network;
using Chromely.Loader;
using Chromely.NativeHosts;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    class RekoBrowserHostController : WindowController
    {
        public RekoBrowserHostController(IChromelyWindow window, IChromelyNativeHost nativeHost, IChromelyConfiguration config, IChromelyRouteProvider routeProvider, IChromelyRequestTaskRunner requestTaskRunner, IChromelyCommandTaskRunner commandTaskRunner, IChromelyRequestSchemeProvider requestSchemeProvider, ChromelyHandlersResolver handlersResolver) : base(window, nativeHost, config, routeProvider, requestTaskRunner, commandTaskRunner, requestSchemeProvider, handlersResolver)
        {
        }

        protected override CefApp CreateApp()
        {
            return new RekoBrowserApp();
        }
    }
}
