using Reko.Chromely.RekoHosting;
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	/// <summary>
	/// Custom RenderProcessHandler used to register global variables and functions
	/// </summary>
	public class RekoRenderProcessHandler : CefRenderProcessHandler
	{
        private readonly PendingPromisesRepository pendingPromises = new PendingPromisesRepository();
        private ServiceContainer services;
        private Decompiler? decompiler;

        public RekoRenderProcessHandler()
        {
            this.services = new ServiceContainer();
        }

		/// <summary>
		/// Register globals once the context is ready
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="frame"></param>
		/// <param name="context"></param>
		protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            CreateRekoInstance(context);

            //$TODO:Expose the Decompiler.Load/Scan/Analyze methods in RekoBrowserGlobals

            new RekoBrowserGlobals(pendingPromises, context).RegisterGlobals();
        }

        private void CreateRekoInstance(CefV8Context context)
        {
            var fsSvc = new FileSystemServiceImpl();
            var listener = new ListenerService(context);
            var diagSvc = new DiagnosticsService(listener, context);
            var dfSvc = new DecompiledFileService(fsSvc);
            services.AddService(typeof(IFileSystemService), fsSvc);
            services.AddService(typeof(DecompilerEventListener), listener);
            var configSvc = RekoConfigurationService.Load(services, "reko/reko.config");
            services.AddService(typeof(IConfigurationService), configSvc);
            services.AddService(typeof(IDiagnosticsService), diagSvc);
            services.AddService(typeof(IDecompiledFileService), dfSvc);
            var loader = new Reko.Loading.Loader(services);
            this.decompiler = new Reko.Decompiler(loader, services);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if(message.Name == "openFileReply")
            {
                var promiseId = message.Arguments.GetInt(0);
                var filePath = message.Arguments.GetString(1);

                var promise = pendingPromises.PopPromise(promiseId);
                if(filePath == null)
                {
                    promise.Reject(null!);
                } else
                {
                    promise.Resolve(filePath);
                }
                return true;
            }
            return false;
        }
    }
}