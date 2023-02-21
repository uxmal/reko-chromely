#region License
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

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.Renderers
{
    using Program = Reko.Core.Program;
    using static System.Net.WebUtility;

    public class ProcedureRenderer : Formatter
    {
        private readonly StringBuilder sb;

        public static string RenderProcedureHll(Program program, string sProcAddress)
        {
            if (!program.Architecture.TryParseAddress(sProcAddress, out var addr))
                return "";
            if (!program.Procedures.TryGetValue(addr, out var proc))
                return "";
            var sb = new StringBuilder();
            var writer = new ProcedureRenderer(sb);
            var ar = new AbsynCodeFormatter(writer);
            ar.Write(proc);
            return sb.ToString();
        }

        public static string RenderProcedureIR(Program program, string sProcAddress)
        {
            if (!program.Architecture.TryParseAddress(sProcAddress, out var addr))
                return "";
            if (!program.Procedures.TryGetValue(addr, out var proc))
                return "";
            var sb = new StringBuilder();

            var writer = new ProcedureRenderer(sb);
            var ar = new AbsynCodeFormatter(writer);
            new ProcedureFormatter(proc, new BlockDecorator { ShowEdges = false }, ar).WriteProcedureBlocks();
            return sb.ToString();
        }

        public ProcedureRenderer(StringBuilder sb)
        {
            this.sb = sb;
        }

        public override void Terminate()
        {
            sb.AppendLine("<br />");
        }

        public override void Write(string s)
        {
            sb.Append(HtmlEncode(s));
        }

        public override Formatter Write(char ch)
        {
            sb.Append(HtmlEncode(new string(ch, 1)));
            return this;
        }

        public override void Write(string format, params object[] arguments)
        {
            var s = string.Format(format, arguments);
            sb.Append(HtmlEncode(s));
        }

        public override void WriteComment(string comment)
        {
            sb.Append("<span class='cmt'>");
            sb.Append("// ");
            sb.Append(HtmlEncode(comment));
            sb.Append("</span>");
        }

        public override void WriteHyperlink(string text, object href)
        {
            sb.Append("<span class='link' ");
            switch (href)
            {
            case Address addr:
                sb.AppendFormat(" data-addr='{0}'", addr);
                break;
            case Procedure proc:
                sb.AppendFormat(" data-proc='{0}'", proc.EntryAddress);
                break;
            }
            sb.Append(">");
            sb.Append(HtmlEncode(text));
            sb.Append("</span>");
        }

        public override void WriteKeyword(string keyword)
        {
            sb.Append("<span class='kw'>");
            sb.Append(HtmlEncode(keyword));
            sb.Append("</span>");
        }

        public override void WriteLine(string format, params object[] arguments)
        {
            sb.Append(HtmlEncode(string.Format(format, arguments)));
            sb.AppendLine("<br />");
        }

        public override void WriteLine()
        {
            sb.AppendLine("<br />");
        }

        public override void WriteLine(string s)
        {
            sb.Append(HtmlEncode(s));
            sb.AppendLine("<br />");
        }

        public override void WriteType(string typeName, DataType dt)
        {
            sb.AppendFormat("<span class='type' data-typename='{0}'>", HtmlEncode(dt.Name));
            sb.Append(HtmlEncode(typeName));
            sb.Append("</span>");
        }

        public override void WriteLabel(string label, object block)
        {
            sb.AppendFormat("<span class='label'>");
            sb.Append(HtmlEncode(label));
            sb.Append("</span>");
        }

        public override void Begin(object? tag)
        {
        }
    }
}