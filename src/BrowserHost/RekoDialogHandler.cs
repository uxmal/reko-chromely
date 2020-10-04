using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    public class RekoDialogHandler : CefDialogHandler
    {
        protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, string[] acceptFilters, int selectedAcceptFilter, CefFileDialogCallback callback)
        {
            return base.OnFileDialog(browser, mode, title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback);
        }
    }
}
