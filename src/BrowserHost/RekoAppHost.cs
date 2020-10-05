using Chromely.CefGlue.Browser;
using Chromely.CefGlue.BrowserWindow;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Host;
using Chromely.Core.Network;
using Chromely.Native;
using Reko.Chromely.BrowserHost.ChromelyExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.BrowserHost
{
    public class RekoAppHost : ExtensibleAppHost
    {
        public RekoAppHost(IChromelyNativeHost nativeHost, IChromelyContainer container, IChromelyConfiguration config, IChromelyRequestTaskRunner requestTaskRunner, IChromelyCommandTaskRunner commandTaskRunner) : base(nativeHost, container, config, requestTaskRunner, commandTaskRunner)
        {
        }

        protected override CefGlueApp CreateApp(IChromelyConfiguration config)
        {
            return new RekoBrowserApp(config);
        }

        protected override IWindow CreateMainView()
        {
            HostRuntime.EnsureNativeHostFileExists(_config);

            if (_mainWindow == null)
            {
                _mainWindow = new RekoBrowserHost(_nativeHost, _container, _config, _commandTaskRunner, BrowserMessageRouter);
            }
            return _mainWindow;
        }
    }
}
