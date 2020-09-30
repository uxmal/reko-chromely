using Chromely;
using Chromely.Browser;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Host;

namespace Reko.Chromely
{
    public class RekoWindow : Window
    {
        public RekoWindow(IChromelyNativeHost nativeHost, IChromelyConfiguration config, ChromelyHandlersResolver handlersResolver) : base(nativeHost, config, handlersResolver)
        {
            this.FrameLoadStart += RekoWindow_FrameLoadStart;
        }

        private void RekoWindow_FrameLoadStart(object? sender, FrameLoadStartEventArgs e)
        {
            var frame = e.Frame.Browser.GetMainFrame();
            var x = _config.JavaScriptExecutor;
            x.ExecuteScript("window.menus = [ 'a', 'b', 'c', 'd' ]");
        }
    }
}
