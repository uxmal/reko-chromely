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

using Chromely.Core;
using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.RekoHosting
{
    public class DiagnosticsService : IDiagnosticsService
    {
        private readonly DecompilerEventListener listener;
        private readonly IChromelyJavaScriptExecutor js;

        public DiagnosticsService(DecompilerEventListener listener, IChromelyJavaScriptExecutor js)
        {
            this.listener = listener;
            this.js = js;
        }

        public void ClearDiagnostics()
        {
            js.ExecuteScript("diagnostics.clear()");
        }

        public void Error(string message)
        {
            listener.Error(new NullCodeLocation(""), message);
        }

        public void Error(string message, params object[] args)
        {
            listener.Error(new NullCodeLocation(""), message, args);
        }

        public void Error(Exception ex, string message)
        {
            listener.Error(new NullCodeLocation(""), ex, message);
        }

        public void Error(ICodeLocation location, string message)
        {
            listener.Error(location, message);
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            listener.Error(location, message, args);
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            listener.Error(location, ex, message);
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            listener.Error(location, ex, message, args);
        }

        public void Inform(string message)
        {
            listener.Info(new NullCodeLocation(""), message);
        }

        public void Inform(string message, params object[] args)
        {
            listener.Info(new NullCodeLocation(""), message, args);
        }

        public void Inform(ICodeLocation location, string message)
        {
            listener.Info(location, message);
        }

        public void Inform(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
