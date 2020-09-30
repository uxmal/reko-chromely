using Caliburn.Light;
using Chromely.CefGlue.Browser;
using Chromely.Core.Configuration;
using Chromely.Core.Defaults;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Reko.Chromely.BrowserHost
{
	public class RekoBrowserHost
	{
		private const string WINDOW_TITLE = "Reko Decompiler";

		private static IChromelyConfiguration CreateConfiguration() {
			return new DefaultConfiguration();
		}

		private static CefSettings CreateSettings() {
			return new CefSettings() {
				MultiThreadedMessageLoop = false,
				NoSandbox = true
			};
		}

		private static CefMessageRouterBrowserSide CreateRouter() {
			var routerConfig = new CefMessageRouterConfig();
			return new CefMessageRouterBrowserSide(routerConfig);
		}
		
		private static CefBrowserSettings CreateBrowserSettings() {
			return new CefBrowserSettings();
		}

		private void InitializeRuntime() {
			CefRuntime.Load();
			var mainArgs = new CefMainArgs(new string[] { });
			// spawn browser process. -1 indicates a browser process has been created
			if(CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero) != -1) {
				throw new Exception("Failed to create browser process");
			}

			var settings = CreateSettings();
			CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);
		}

		/// <summary>
		/// Creates the window construction parameters
		/// <see cref="CefWindowInfo"/> describes how we want the window to be created
		/// </summary>
		/// <returns></returns>
		private static CefWindowInfo CreateWindowInfo() {
			var windowInfo = CefWindowInfo.Create();
			windowInfo.SetAsPopup(IntPtr.Zero, WINDOW_TITLE);
			return windowInfo;
		}

		private CefGlueBrowser CreateBrowserObject() {
			var router = CreateRouter();

			var container = new SimpleContainer();
			var taskRunner = new DefaultCommandTaskRunner(container);

			var browserSettings = CreateBrowserSettings();
			return new CefGlueBrowser(this, container, config, taskRunner, router, browserSettings);
		}

		private readonly IChromelyConfiguration config;
		private readonly RekoBrowserApp app;

		private CefGlueBrowser? browser;
		private CefBrowserHost? host;
		private bool running = false;

		private readonly string initialUrl;

		public RekoBrowserHost() {
			this.config = CreateConfiguration();
			this.app = new RekoBrowserApp(config);
			InitializeRuntime();

			// Obtain the html url relative to the executing assembly
			var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			initialUrl = Path.Combine(baseDirectory!, "app", "index.html");
		}

		/// <summary>
		/// Create the browser and start the blocking message loop
		/// </summary>
		public void Start() {
			this.browser = CreateBrowserObject();

			browser.Created += Browser_Created;
			browser.BeforeClose += Browser_BeforeClose;

			var window = CreateWindowInfo();
			browser.StartUrl = initialUrl;
			browser.Create(window);

			running = true;
			while (running) {
				CefRuntime.DoMessageLoopWork();
			}

			host!.CloseBrowser(true);
			CefRuntime.Shutdown();
		}

		private void Browser_BeforeClose(object? sender, global::Chromely.CefGlue.Browser.EventParams.BeforeCloseEventArgs e) {
			this.running = false;
		}

		/// <summary>
		/// Saves the host locally as soon as the browser has been created
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Browser_Created(object? sender, EventArgs e) {
			var browser = sender as CefGlueBrowser;
			host = browser!.CefBrowser.GetHost();
		}
	}
}
