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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost
{
    /// <summary>
    /// This class is responsible for injecting the 'reko' global object into the global JS context of the caller.
    /// The 'rek
    /// </summary>
	public class RekoBrowserGlobals
	{
        /// <summary>
        /// Add a synchronous method to the supplied JavaScript object.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="jsObject"></param>
        /// <param name="func"></param>
        private void RegisterFunction(string functionName, CefV8Value jsObject, Func<CefV8Value[],CefV8Value> func)
        {
            var handler = CefV8Value.CreateFunction(functionName, new HandlerProxy(func));
            jsObject.SetValue(functionName, handler);
        }

        /// <summary>
        /// Add an asynchronous method to the supplied JavaScript object
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="jsObject"></param>
        /// <param name="func"></param>
        private void RegisterAsyncFunction(string functionName, CefV8Value jsObject, Func<CefV8Value[], CefV8Value> func)
        {
            var handler = CefV8Value.CreateFunction(functionName, new AsyncHandlerProxy(func));
            jsObject.SetValue(functionName, handler);
        }


        /// <summary>
        /// Register custom variables and functions
        /// in the global context
        /// </summary>
        /// <param name="context"></param>
        public void RegisterGlobals(CefV8Context context)
        {
			context.Acquire(() => {
				var glbl = context.GetGlobal();
				var rekoObj = CefV8Value.CreateObject();
				glbl.SetValue("reko", rekoObj);
				//RegisterFunction<Proto_DisassembleRandomBytes>("Proto_DisassembleRandomBytes", rekoObj);
				RegisterAsyncFunction("Proto_DisassembleRandomBytes", rekoObj, Proto_DisassembleRandomBytes.Execute);
			});
		}
	}
}
