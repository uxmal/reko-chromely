using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	/// <summary>
	/// Custom RenderProcessHandler used to register global variables and functions
	/// </summary>
	public class RekoRenderProcessHandler : CefRenderProcessHandler
	{
		public RekoRenderProcessHandler() {
		}

		/// <summary>
		/// Register globals once the context is ready
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="frame"></param>
		/// <param name="context"></param>
		protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context) {
			new RekoBrowserGlobals().RegisterGlobals(context);
		}
	}
}