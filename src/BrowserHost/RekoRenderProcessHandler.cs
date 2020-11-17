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

using Reko.Chromely.RekoHosting;
using Reko.Core;
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
        private readonly ServiceContainer services = new ServiceContainer();
        private Decompiler? decompiler;
        private readonly EventListenersRepository eventListeners = new EventListenersRepository();

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
            if (this.decompiler == null)
            {
                decompiler = CreateRekoInstance(context);
            }

            // attach and register events
            var listener = (ListenerService) services.RequireService<DecompilerEventListener>();
            listener.SetContext(context);

            new RekoBrowserGlobals(pendingPromises!, eventListeners!, services, this.decompiler!, context).RegisterGlobals();
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            // wipe pending promises and events
            this.pendingPromises?.Reset();
            this.eventListeners?.Reset();
        }

        private Reko.Decompiler CreateRekoInstance(CefV8Context context)
        {
            var fsSvc = new FileSystemServiceImpl();
            var listener = new ListenerService(context, eventListeners);
            var dfSvc = new DecompiledFileService(fsSvc, listener);
            services.AddService(typeof(IFileSystemService), fsSvc);
            services.AddService(typeof(DecompilerEventListener), listener);
            var configSvc = RekoConfigurationService.Load(services, "reko/reko.config");
            services.AddService(typeof(IConfigurationService), configSvc);
            services.AddService(typeof(IDecompiledFileService), dfSvc);
            services.AddService(typeof(ITypeLibraryLoaderService), new TypeLibraryLoaderServiceImpl(services));
            var loader = new Reko.Loading.Loader(services);
            return new Reko.Decompiler(loader, services);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (message.Name == "openFileReply")
            {
                var promiseId = message.Arguments.GetInt(0);
                var filePath = message.Arguments.GetString(1);

                var promise = pendingPromises.RemovePromise(promiseId);
                if (filePath is null)
                {
                    promise.Reject(null!);
                }
                else
                {
                    promise.Resolve(filePath);
                }
                return true;
            }
            return false;
        }
    }
}