using System;
using System.Collections.Generic;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	/// <summary>
	/// Custom RenderProcessHandler used to register global variables and functions
	/// </summary>
	public class RekoRenderProcessHandler : CefRenderProcessHandler
	{
        private readonly PendingPromisesRepository pendingPromises = new PendingPromisesRepository();

		/// <summary>
		/// Register globals once the context is ready
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="frame"></param>
		/// <param name="context"></param>
		protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context) {
			new RekoBrowserGlobals(pendingPromises, context).RegisterGlobals();
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