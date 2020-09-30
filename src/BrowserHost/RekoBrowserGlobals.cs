using Reko.Chromely.BrowserHost.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	public class RekoBrowserGlobals
	{
		private void RegisterFunction<T>(string functionName, CefV8Value global) where T : CefV8Handler, new() {
			var fn = CefV8Value.CreateFunction(functionName, new T());
			global.SetValue(functionName, fn);
		}

		public void RegisterGlobals(CefV8Context context) {
			context.Enter();

			var glbl = context.GetGlobal();
			RegisterFunction<ExecuteJavascript>("ExecuteJavascript", glbl);

			context.Exit();
		}
	}
}
