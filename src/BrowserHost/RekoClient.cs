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

using Chromely;
using Chromely.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
    public class RekoClient : CefBrowserClient
    {
        public RekoClient(CefMessageRouterBrowserSide browserMessageRouter, ChromelyHandlersResolver handlersResolver) : base(browserMessageRouter, handlersResolver)
        {
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if(message.Name == "openFileRequest")
            {
                var host = browser.GetHost();

                var promiseId = message.Arguments.GetInt(0);
                var cb = new RekoOpenFileDialogCallback(browser, promiseId);
                host.RunFileDialog(CefFileDialogMode.Open, "Select a file to decompile", null, null, 0, cb);
                return true;
            }

            return base.OnProcessMessageReceived(browser, frame, sourceProcess, message);
        }
    }
}
