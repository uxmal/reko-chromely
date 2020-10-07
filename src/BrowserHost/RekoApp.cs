using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Defaults;
using Chromely.Core.Host;
using Chromely.Core.Infrastructure;
using Chromely.Core.Network;
using Chromely.NativeHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.BrowserHost
{
    public class RekoApp : ChromelyBasicApp
    {
        public override void ConfigureServices(ServiceCollection services)
        {
            base.ConfigureServices(services);
            services.TryAddSingleton<ChromelyWindowController, RekoBrowserHostController>();
        }
    }
}
