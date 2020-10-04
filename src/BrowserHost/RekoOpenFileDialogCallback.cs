using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    class RekoOpenFileDialogCallback : CefRunFileDialogCallback
    {
        private readonly CefBrowser browser;
        private readonly int promiseId;

        public RekoOpenFileDialogCallback(CefBrowser browser, int promiseId)
        {
            this.browser = browser;
            this.promiseId = promiseId;
        }

        protected override void OnFileDialogDismissed(int selectedAcceptFilter, string[] filePaths)
        {
            var msg = CefProcessMessage.Create("openFileReply");
            msg.Arguments.SetInt(0, promiseId);

            if (filePaths == null || filePaths.Length < 1)
            {
                msg.Arguments.SetString(1, null);
            }
            else
            {
                msg.Arguments.SetString(1, filePaths.First());
            }

            browser.GetMainFrame().SendProcessMessage(CefProcessId.Renderer, msg);
        }
    }
}
