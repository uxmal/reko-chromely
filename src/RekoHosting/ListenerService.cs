﻿#region
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
using Reko.Chromely.BrowserHost;
using Reko.Core;
using Reko.Core.Output;
using Reko.Core.Scripts;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Reko.Chromely.RekoHosting
{

    public abstract class MessageListenerSink
    {
        public const string EV_ERROR = "Listener.Error";
        public const string EV_INFO = "Listener.Info";
        public const string EV_WARN = "Listener.Warn";
    }

    internal class PostMessageTask : CefTask
    {
        private readonly EventListenersRepository eventListeners;
        private readonly string sinkName;

        private readonly CefV8Context ctx;
        private readonly string message;

        public PostMessageTask(CefV8Context ctx, EventListenersRepository eventListeners, string sinkName, string message)
        {
            this.ctx = ctx;
            this.eventListeners = eventListeners;
            this.sinkName = sinkName;
            this.message = message;
        }

        protected override void Execute()
        {
            ctx.Acquire(() =>
            {
                eventListeners.Invoke(sinkName, new CefV8Value[]
                {
                    CefV8Value.CreateString(message)
                });
            });
        }
    }

    //$TODO: this needs to call back to JavaScript to show diagnostics. We want to call back to JavaScript, somehow obtaining
    // a reference to the current context. Or rather, get the runner and push a task on it. right?
    public class ListenerService : DecompilerEventListener
    {
        private bool canceled;
        private CefV8Context ctx;

        private readonly EventListenersRepository eventListeners;

        public IProgressIndicator Progress
        {
            get
            {
                return NullProgressIndicator.Instance;
            }
        }

        public ListenerService(CefV8Context ctx, EventListenersRepository eventListeners)
        {
            this.eventListeners = eventListeners;
            this.ctx = ctx;
            SetContext(ctx);
        }

        public void SetContext(CefV8Context ctx)
        {
            this.ctx = ctx;
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            eventListeners.RegisterEvent(MessageListenerSink.EV_ERROR);
            eventListeners.RegisterEvent(MessageListenerSink.EV_INFO);
            eventListeners.RegisterEvent(MessageListenerSink.EV_WARN);
        }

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
            //JavaScript.ExecuteScript($"diagnostics.advance({count})");
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
            return new JsLocation(@$"{{""program"":""{program.Name}"",""stmLoc"":""{stm.Address}""}}");
        }

        public void Error(string message)
        {
            Error(new NullCodeLocation(""), message);
        }

        public void Error(string format, params object[] args)
        {
            Error(new NullCodeLocation(""), string.Format(format, args));
        }

        public void Error(Exception ex, string message)
        {
            Error(new NullCodeLocation(""), ex, message);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Error(new NullCodeLocation(""), ex, string.Format(format, args));
        }

        public void Error(ICodeLocation location, string message)
        {
            ctx.GetTaskRunner().PostTask(new PostMessageTask(
                ctx, eventListeners,
                MessageListenerSink.EV_ERROR, $"<div class='diag-err'><div>NYI</div><div>{message}</div></div>"));
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

        public void Info(string message)
        {
            Info(new NullCodeLocation(""), message);
        }

        public void Info(string format, params object[] args)
        {
            Info(new NullCodeLocation(""), string.Format(format, args));
        }

        public void Info(ICodeLocation location, string message)
        {
            ctx.GetTaskRunner().PostTask(new PostMessageTask(
                ctx, eventListeners,
                MessageListenerSink.EV_INFO, $"<div class='diag-inf'><div>NYI</div><div>{message}</div></div>"));
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
            //JavaScript.ExecuteScript($"diagnostics.progress({Quote(caption)}, {numerator}, {denominator})");
        }

        public void ShowStatus(string caption)
        {
            //JavaScript.ExecuteScript($"diagnostics.status({Quote(caption)})");
        }


        public void Warn(string message)
        {
            Error(new NullCodeLocation(""), message);
        }

        public void Warn(string format, params object[] args)
        {
            Error(new NullCodeLocation(""), string.Format(format, args));
        }

        public void Error(ScriptError scriptError)
        {
            Error(new NullCodeLocation(scriptError.FileName), $"{scriptError.LineNumber}:{scriptError.Message}");
        }

        public void Warn(ICodeLocation location, string message)
        {
            ctx.GetTaskRunner().PostTask(new PostMessageTask(
                ctx, eventListeners,
                MessageListenerSink.EV_WARN, $"<div class='diag-warn'><div>NYI</div><div>{message}</div></div>"));
        }

        public void Warn(ICodeLocation location, string message, params object[] args)
        {
            Warn(location, string.Format(message, args));
        }

        public class JsLocation : ICodeLocation
        {
            public JsLocation(string jsonText)
            {
                this.Text = jsonText;
            }

            public string Text { get; }

            public ValueTask NavigateTo()
            {
                throw new NotImplementedException();
            }
        }

        private class DiagnosticTask : CefTask
        {
            private CefV8Context ctx;
            private string sHtml;

            public DiagnosticTask(CefV8Context ctx, string sHtml)
            {
                this.ctx = ctx;
                this.sHtml = sHtml;
            }

            protected override void Execute()
            {
                ctx.Acquire(() =>
                {
                    //$TODO: smx-smx call back to the JS side, passing a blob of html
                });
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

        public ICodeLocation CreateAddressNavigator(IReadOnlyProgram program, Address address)
        {
            return new NullCodeLocation("");
        }

        public ICodeLocation CreateProcedureNavigator(IReadOnlyProgram program, Procedure proc)
        {
            return new NullCodeLocation("");
        }

        public ICodeLocation CreateBlockNavigator(IReadOnlyProgram program, Block block)
        {
            return new NullCodeLocation("");
        }

        public ICodeLocation CreateStatementNavigator(IReadOnlyProgram program, Statement stm)
        {
            return new NullCodeLocation("");
        }

        public ICodeLocation CreateJumpTableNavigator(IReadOnlyProgram program, IProcessorArchitecture arch, Address addrIndirectJump, Address? addrVector, int stride)
        {
            return new NullCodeLocation("");
        }
    }
}
