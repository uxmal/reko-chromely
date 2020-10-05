using Chromely.CefGlue.Browser;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
    public class RekoClient : CefGlueClient
    {
        private readonly CefDialogHandler dialogHandler = new RekoDialogHandler();

        public RekoClient(CefMessageRouterBrowserSide browserMessageRouter, CefGlueCustomHandlers handlers) : base(browserMessageRouter, handlers)
        {
        }

        //protected override CefDialogHandler GetDialogHandler()
        //{
        //    return dialogHandler;
        //}

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

            return false;
        }
    }
}
