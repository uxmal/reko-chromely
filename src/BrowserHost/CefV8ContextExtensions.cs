using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	public static class CefV8ContextExtensions
	{
		public static void Acquire(this CefV8Context ctx, Action body) {
			ctx.Enter();
			body.Invoke();
			ctx.Exit();
		}
	}
}
