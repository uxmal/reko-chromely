#region
// Copyright 2020 the Reko contributors.

// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the
// 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
#endregion

using Reko.Chromely.BrowserHost.Functions;
using Reko.Chromely.Renderers;
using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    /// <summary>
    /// This class is responsible for injecting the 'reko' global object into the global JS context of the caller.
    /// </summary>
	public class RekoBrowserGlobals
	{
        private readonly CefV8Context context;
        private readonly CefPromiseFactory promiseFactory;
        private readonly PendingPromisesRepository pendingPromises;
        private readonly EventListenersRepository eventListeners;
        private readonly ServiceContainer services;
        private readonly IDecompiler decompiler;

        public RekoBrowserGlobals(
            PendingPromisesRepository pendingPromises,
            EventListenersRepository eventListeners,
            ServiceContainer services,
            IDecompiler decompiler,
            CefV8Context context
        )
        {
            this.pendingPromises = pendingPromises;
            this.eventListeners = eventListeners;
            this.services = services;
            this.decompiler = decompiler;
            this.context = context;
            CefPromiseFactory promiseFactory = CreatePromiseFactory();
            this.promiseFactory = promiseFactory;
        }

        /// <summary>
        /// Creates a "createPromise" function on the JS side.
        /// </summary>
        private CefPromiseFactory CreatePromiseFactory()
        {
            context.Enter();
            context.GetFrame().ExecuteJavaScript(@"
                window.createPromise = function (fn) {
                    return new Promise(fn);
                };
                ", null, 1);

            var global = context.GetGlobal();
            var promiseFactoryFn = global.GetValue("createPromise");

            var promiseFactory = new CefPromiseFactory(promiseFactoryFn);
            context.Exit();
            return promiseFactory;
        }

        /// <summary>
        /// Add a synchronous method to the supplied JavaScript object.
        /// </summary>
        /// <param name="jsObject"></param>
        /// <param name="functionName"></param>
        /// <param name="func"></param>
        private void RegisterFunction(CefV8Value jsObject, string functionName, Func<CefV8Value[], CefV8Value> func)
        {
            var handler = CefV8Value.CreateFunction(functionName, new HandlerProxy(func));
            jsObject.SetValue(functionName, handler);
        }

        private void RegisterFunction(CefV8Value jsObject, string functionName, CefV8Handler handler)
        {
            jsObject.SetValue(functionName, CefV8Value.CreateFunction(functionName, handler));
        }

        private void RegisterFunction(CefV8Value jsObject, string functionName, Delegate func)
        {
            jsObject.SetValue(functionName, CefV8Value.CreateFunction(functionName, new HandlerProxy(func)));
        }

        /// <summary>
        /// Add an asynchronous method to the supplied JavaScript object
        /// </summary>
        /// <param name="jsObject"></param>
        /// <param name="functionName"></param>
        /// <param name="func"></param>
        private void RegisterAsyncFunction(CefV8Value jsObject, string functionName, Delegate func)
        {
            var handler = CefV8Value.CreateFunction(functionName, new AsyncHandlerProxy(func, this.promiseFactory));
            jsObject.SetValue(functionName, handler);
        }

        private void RegisterAsyncFunction(CefV8Value jsObject, string functionName, CefV8Handler handler)
        {
            jsObject.SetValue(functionName, CefV8Value.CreateFunction(functionName, handler));
        }

        /// <summary>
        /// Register custom variables and functions as the object 'reko' in the global context
        /// </summary>
        public void RegisterGlobals()
        {
			context.Acquire(() => {

                var global = context.GetGlobal();
                var rekoObj = CefV8Value.CreateObject();

                global.SetValue("reko", rekoObj);
                RegisterAsyncFunction(rekoObj, "OpenFile", new OpenFileHandler(promiseFactory, pendingPromises));
				RegisterAsyncFunction(rekoObj, "Proto_DisassembleRandomBytes", new Func<string,string,string>(Proto_DisassembleRandomBytes.Execute));
                RegisterAsyncFunction(rekoObj, "Proto_GeneratePng", new Func<int,byte[]>(Proto_GeneratePng.Execute));
                RegisterAsyncFunction(rekoObj, "LoadFile", new Func<string, string, bool>(decompiler.Load));
                RegisterAsyncFunction(rekoObj, "Scan", new Action(decompiler.ScanPrograms));
                RegisterFunction(rekoObj, "TestListener", new Action(() =>
                {
                    var listener = services.RequireService<DecompilerEventListener>();
                    listener.Info(new NullCodeLocation("web"), "Hello World");
                }));
                RegisterFunction(rekoObj, "RegisterEventListener", new Proto_RegisterEventListener(eventListeners));
                RegisterFunction(rekoObj, "RenderProjectView", new Func<string>(() => ProjectViewRenderer.RenderToHtml(decompiler)));
            });
		}
	}
}
