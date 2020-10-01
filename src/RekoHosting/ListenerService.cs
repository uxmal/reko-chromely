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
using Chromely.Core.Configuration;
using Reko.Core;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.RekoHosting
{
    public class ListenerService : DecompilerEventListener
    {
        private readonly IChromelyConfiguration config;
        private bool canceled;

        public ListenerService(IChromelyConfiguration config)
        {
            this.config = config;
        }

        private IChromelyJavaScriptExecutor JavaScript => config.JavaScriptExecutor;

        public void Cancel()
        {
            this.canceled = true;
        }

        public void ResetCancel()
        {
            this.canceled = false;
        }
        public void Advance(int count)
        {
            JavaScript.ExecuteScript($"diagnostics.advance({count})");
        }

        public ICodeLocation CreateAddressNavigator(Core.Program program, Address address)
        {
            return new JsLocation(@$"{{""program"":""{program.Name}"",""addr"":""{address}""}}");
        }

        public ICodeLocation CreateBlockNavigator(Core.Program program, Block block)
        {
            return new JsLocation(@$"{{""program"":""{program.Name}"",""blockAddr"":""{block.Address}""}}");
        }

        public ICodeLocation CreateJumpTableNavigator(Core.Program program, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride)
        {
            throw new NotImplementedException();
        }

        public ICodeLocation CreateProcedureNavigator(Core.Program program, Procedure proc)
        {
            return new JsLocation(@$"{{""program"":""{program.Name}"",""procAddr"":""{proc.EntryAddress}""}}");
        }

        public ICodeLocation CreateStatementNavigator(Core.Program program, Statement stm)
        {
            return new JsLocation(@$"{{""program"":""{program.Name}"",""stmLoc"":""{stm.LinearAddress}""}}");
        }

        public void Error(ICodeLocation location, string message)
        {
            JavaScript.ExecuteScript($"diagnostics.error({Quote(location.Text)},{Quote(message)})");
        }

        public void Error(ICodeLocation location, string message, params object[] args)
        {
            Error(location, string.Format(message, args));
        }

        public void Error(ICodeLocation location, Exception ex, string message)
        {
            Error(location, string.Format("{0} {1}", ex.Message, message));
        }

        public void Error(ICodeLocation location, Exception ex, string message, params object[] args)
        {
            Error(location, ex, string.Format(message, args));
        }

        public void Info(ICodeLocation location, string message)
        {
            JavaScript.ExecuteScript($"diagnostics.info({Quote(location.Text)},{Quote(message)})");
        }

        public void Info(ICodeLocation location, string message, params object[] args)
        {
            Info(location, string.Format(message, args));
        }

        public bool IsCanceled()
        {
            return this.canceled;
        }

        public void ShowProgress(string caption, int numerator, int denominator)
        {
            JavaScript.ExecuteScript($"diagnostics.progress({Quote(caption)}, {numerator}, {denominator})");
        }

        public void ShowStatus(string caption)
        {
            JavaScript.ExecuteScript($"diagnostics.status({Quote(caption)})");
        }

        public void Warn(ICodeLocation location, string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public class JsLocation : ICodeLocation
        {
            public JsLocation(string jsonText)
            {
                this.Text = jsonText;
            }

            public string Text { get; }

            public void NavigateTo()
            {
                throw new NotSupportedException();
            }
        }

        private string Quote(string s)
        {
            var sb = new StringBuilder();
            sb.Append('"');
            foreach (var ch in s)
            {
                switch (ch)
                {
                case '"': sb.Append(@"\"""); break;
                case '\\': sb.Append(@"\\"); break;
                default: sb.Append(ch); break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

    }
}
