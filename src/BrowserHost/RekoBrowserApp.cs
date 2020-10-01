﻿using Chromely.CefGlue.Browser;
using Chromely.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
	public class RekoBrowserApp : CefGlueApp
	{
		private readonly RekoRenderProcessHandler renderProcessHandler = new RekoRenderProcessHandler();

		public RekoBrowserApp(IChromelyConfiguration config) : base(config) {
		}

		/// <summary>
		/// Override that provides our custom <see cref="RekoRenderProcessHandler" />
		/// </summary>
		/// <returns></returns>
		protected override CefRenderProcessHandler GetRenderProcessHandler() {
			return renderProcessHandler;
		}
	}
}