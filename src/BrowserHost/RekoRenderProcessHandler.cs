using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	public class RekoRenderProcessHandler : CefRenderProcessHandler
	{
		public RekoRenderProcessHandler() {
		}

		protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context) {
			new RekoBrowserGlobals().RegisterGlobals(context);
		}
	}
}