using Chromely.CefGlue.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost.ChromelyExtensions
{
    public class ExtensibleGlueBrowser : CefGlueBrowser
    {
        public ExtensibleGlueBrowser(object owner, IChromelyContainer container, IChromelyConfiguration config, IChromelyCommandTaskRunner commandTaskRunner, CefMessageRouterBrowserSide browserMessageRouter, CefBrowserSettings settings) : base(owner, container, config, commandTaskRunner, browserMessageRouter, settings)
        {
        }

        public new virtual void Create(CefWindowInfo windowInfo)
        {
            base.Create(windowInfo);
        }
    }
}
