using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    /// <summary>
    /// Extension methods for the <see cref="CefV8Context"/> class.
    /// </summary>
	public static class CefV8ContextExtensions
	{
        public static void Acquire(this CefV8Context ctx, Action body)
        {
            ctx.Enter();
            try
            {
                body.Invoke();
            }
            finally
            {
                ctx.Exit();
            }
        }
	}
}
