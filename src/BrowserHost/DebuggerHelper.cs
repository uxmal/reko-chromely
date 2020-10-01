using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Reko.Chromely.BrowserHost
{
    public class DebuggerHelper
    {
        public static void Launch()
        {
            Debugger.Launch();
            while (!Debugger.IsAttached) Thread.Sleep(200);
            Debugger.Break();
        }
    }
}
