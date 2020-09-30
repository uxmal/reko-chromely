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
using Chromely.Core;
using Chromely.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Reko.Chromely
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var config = DefaultConfiguration.CreateForRuntimePlatform();
            //config.StartUrl = "https://google.com";
            config.StartUrl = "local://app/index.html";

            AppBuilder.Create()
                .UseApp<RekoApp>()
                .UseConfig<DefaultConfiguration>(config)
                .UseWindow<RekoWindow>()
                .Build()
                .Run(args);
        }
    }

    public class RekoApp : ChromelyBasicApp
    {
        public RekoApp()
        {
        }

        public override void ConfigureServices(ServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddSingleton<Xilium.CefGlue.CefDisplayHandler, RekoDisplayHandler>();
            RegisterControllerAssembly(services, typeof(RekoApp).Assembly);
        }
    }

    public class RekoDisplayHandler : Xilium.CefGlue.CefDisplayHandler
    {
        public RekoDisplayHandler()
        {
        }
    }

}
