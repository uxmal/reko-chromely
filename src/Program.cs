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
using Chromely.CefGlue.BrowserWindow;
using Chromely.Core.Configuration;
using Chromely.Core.Defaults;
using Chromely.Native;
using Reko.Chromely.BrowserHost;
using System;
using System.IO;
using System.Reflection;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely
{
	class Program
    {
        private static IChromelyConfiguration CreateConfiguration()
        {
            var config = new DefaultConfiguration();

            // Obtain the html url relative to the executing assembly
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var initialUrl = Path.Combine(baseDirectory!, "app", "index.html");

            config.StartUrl = initialUrl;
            return config;
        }

        [STAThread]
        static int Main(string[] args)
        {
            var config = CreateConfiguration();
            var nativeHost = NativeHostFactory.GetNativeHost(config);
            var container = new SimpleContainer();
            var requestTaskRunner = new DefaultRequestTaskRunner(container, config);
            var commandTaskRunner = new DefaultCommandTaskRunner(container);


            var appHost = new RekoAppHost(nativeHost, container, config, requestTaskRunner, commandTaskRunner);
            appHost.Run(args);
            return 0;
        }
    }

}
