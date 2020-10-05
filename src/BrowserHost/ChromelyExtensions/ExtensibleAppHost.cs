using Chromely.CefGlue;
using Chromely.CefGlue.Browser;
using Chromely.CefGlue.BrowserWindow;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Host;
using Chromely.Core.Logging;
using Chromely.Core.Network;
using Chromely.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost.ChromelyExtensions
{
    public class ExtensibleAppHost : HostBase
    {
        public ExtensibleAppHost(IChromelyNativeHost nativeHost, IChromelyContainer container, IChromelyConfiguration config, IChromelyRequestTaskRunner requestTaskRunner, IChromelyCommandTaskRunner commandTaskRunner) : base(nativeHost, container, config, requestTaskRunner, commandTaskRunner)
        {
        }

        protected override IWindow CreateMainView()
        {
            HostRuntime.EnsureNativeHostFileExists(_config);

            if (_mainWindow == null)
            {
                _mainWindow = new ExtensibleBrowserHost(_nativeHost, _container, _config, _commandTaskRunner, BrowserMessageRouter);
            }
            return _mainWindow;
        }

        private static void PostTask(CefThreadId threadId, Action action)
        {
            CefRuntime.PostTask(threadId, new ActionTask(action));
        }

        private void CreateMainWindow()
        {
            if (_config.Platform != ChromelyPlatform.Windows)
            {
                if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
                {
                    PostTask(CefThreadId.UI, CreateMainWindow);
                    return;
                }
            }

            _mainWindow = CreateMainView();
        }

        /// <summary>
        /// The shutdown.
        /// </summary>
        private void Shutdown()
        {
            ExitWindow();
        }

        protected virtual CefGlueApp CreateApp(IChromelyConfiguration config)
        {
            return new CefGlueApp(config);
        }

        public new int Run(string[] args)
        {
            this.Initialize();
            _config.ChromelyVersion = CefRuntime.ChromeVersion;

            var tempFiles = CefBinariesLoader.Load(_config);

            CefRuntime.EnableHighDpiSupport();

            var settings = new CefSettings
            {
                MultiThreadedMessageLoop = _config.Platform == ChromelyPlatform.Windows,
                LogSeverity = CefLogSeverity.Info,
                LogFile = "logs\\chromely.cef_" + DateTime.Now.ToString("yyyyMMdd") + ".log",
                ResourcesDirPath = _config.AppExeLocation
            };

            if (_config.WindowOptions.WindowFrameless || _config.WindowOptions.KioskMode)
            {
                // MultiThreadedMessageLoop is not allowed to be used as it will break frameless mode
                settings.MultiThreadedMessageLoop = false;
            }

            settings.LocalesDirPath = Path.Combine(settings.ResourcesDirPath, "locales");
            settings.RemoteDebuggingPort = 20480;
            settings.Locale = "en-US";
            settings.NoSandbox = true;

            var argv = args;
            if (CefRuntime.Platform != CefRuntimePlatform.Windows)
            {
                argv = new string[args.Length + 1];
                Array.Copy(args, 0, argv, 1, args.Length);
                argv[0] = "-";
            }

            // Update configuration settings
            settings.Update(_config.CustomSettings);

            // Set DevTools url
            _config.DevToolsUrl = $"http://127.0.0.1:{settings.RemoteDebuggingPort}";

            var mainArgs = new CefMainArgs(argv);
            CefApp app = CreateApp(Config);

            if (ClientAppUtils.ExecuteProcess(_config.Platform, argv))
            {
                // CEF applications have multiple sub-processes (render, plugin, GPU, etc)
                // that share the same executable. This function checks the command-line and,
                // if this is a sub-process, executes the appropriate logic.
                var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
                if (exitCode >= 0)
                {
                    // The sub-process has completed so return here.
                    Logger.Instance.Log.Info($"Sub process executes successfully with code: {exitCode}");
                    return exitCode;
                }
            }

            CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);

            /*ScanAssemblies();
            RegisterRoutes();
            RegisterMessageRouters();
            RegisterResourceHandlers();
            RegisterSchemeHandlers();
            RegisterAjaxSchemeHandlers();*/

            CefBinariesLoader.DeleteTempFiles(tempFiles);

            CreateMainWindow();

            Run();

            _mainWindow.Dispose();
            _mainWindow = null;

            Shutdown();

            return 0;
        }

        /// The platform initialize.
        /// </summary>
        protected override void Initialize()
        {
            HostRuntime.LoadNativeHostFile(_config);
        }

        /// <summary>
        /// The platform quit message loop.
        /// </summary>
        protected override void Run()
        {
            _mainWindow?.Run();
        }

        /// <summary>
        /// The platform quit message loop.
        /// </summary>
        protected override void ExitWindow()
        {
            _mainWindow?.Exit();
        }
    }
}
