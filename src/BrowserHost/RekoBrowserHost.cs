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

using Caliburn.Light;
using Chromely.CefGlue.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Defaults;
using Chromely.Core.Host;
using Chromely.Core.Network;
using Chromely.Native;
using Chromely.Windows;
using Reko.Chromely.BrowserHost.ChromelyExtensions;
using System;
using System.IO;
using System.Reflection;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
    public class RekoBrowserHost : ExtensibleBrowserHost
	{
        protected override ExtensibleGlueBrowser CreateBrowser()
        {
            return new RekoGlueBrowser(this, _container, _config, _commandTaskRunner, _browserMessageRouter, new CefBrowserSettings());
        }
        
        public RekoBrowserHost(IChromelyNativeHost nativeHost, IChromelyContainer container, IChromelyConfiguration config, IChromelyCommandTaskRunner commandTaskRunner, CefMessageRouterBrowserSide browserMessageRouter) : base(nativeHost, container, config, commandTaskRunner, browserMessageRouter)
        {
            CefRuntime.RegisterSchemeHandlerFactory("reko", "", new RekoSchemeHandlerFactory());
        }
    }
}
