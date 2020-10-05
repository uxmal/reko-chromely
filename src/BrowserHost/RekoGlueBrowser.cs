#region
// Copyright 2020 the Reko contributors.

// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the
// 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
#endregion

using Chromely.CefGlue.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Network;
using Reko.Chromely.BrowserHost.ChromelyExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
    public class RekoGlueBrowser : ExtensibleGlueBrowser
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

        public override void Create(CefWindowInfo windowInfo)
        {
            settings.DefaultEncoding = "UTF-8";
            settings.FileAccessFromFileUrls = CefState.Enabled;
            settings.UniversalAccessFromFileUrls = CefState.Enabled;
            settings.WebSecurity = CefState.Disabled;
            CefBrowserHost.CreateBrowser(windowInfo, client, settings, StartUrl);
        }
    }
}
