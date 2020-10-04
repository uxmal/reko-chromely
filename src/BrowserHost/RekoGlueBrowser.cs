using Chromely.CefGlue.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
    public class RekoGlueBrowser : CefGlueBrowser
    {
        private readonly CefClient client;
        private readonly CefBrowserSettings settings;

        private CefClient CreateClient(
            IChromelyContainer container,
            IChromelyConfiguration config, IChromelyCommandTaskRunner commandTaskRunner,
            CefMessageRouterBrowserSide browserMessageRouter
        )
        {
            var handlers = CefGlueCustomHandlers.Parse(container, config, commandTaskRunner, this);
            return new RekoClient(browserMessageRouter, handlers);
        }

        public RekoGlueBrowser(
            object owner, IChromelyContainer container,
            IChromelyConfiguration config, IChromelyCommandTaskRunner commandTaskRunner,
            CefMessageRouterBrowserSide browserMessageRouter, CefBrowserSettings settings
        ) : base(owner, container, config, commandTaskRunner, browserMessageRouter, settings)
        {
            this.settings = settings;
            this.client = CreateClient(container, config, commandTaskRunner, browserMessageRouter);
        }

        public new void Create(CefWindowInfo windowInfo)
        {
            settings.DefaultEncoding = "UTF-8";
            settings.FileAccessFromFileUrls = CefState.Enabled;
            settings.UniversalAccessFromFileUrls = CefState.Enabled;
            settings.WebSecurity = CefState.Disabled;
            CefBrowserHost.CreateBrowser(windowInfo, client, settings, StartUrl);
        }

    }
}
