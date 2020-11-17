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
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.Renderers
{
    using Program = Reko.Core.Program;
    public class ProjectViewRenderer
    {
        public static string RenderToHtml(IDecompiler? decompiler)
        {
            var sb = new StringBuilder();
            if (decompiler != null && decompiler.Project != null)
            {
                foreach (var program in decompiler.Project.Programs)
                {
                    RenderProgramNode(program, sb);
                }
            }
            else
            {
                RenderEmptyProject(sb);
            }
            return sb.ToString();
        }

        private static void RenderSegmentNode(Program program, ImageSegment segment, StringBuilder sb)
        {
            sb.AppendFormat("<div class='segment hoverable' data-program='{0}' data-name='{1}' data-addr='{2}'>",
                program.Name, segment.Name, segment.Address);
            sb.AppendFormat("<span class='name'>{0}</span><span class='addr'>{1}</span>", segment.Name, segment.Address);
            sb.AppendLine("</div>");
        }

        private static void RenderEmptyProject(StringBuilder sb)
        {
            sb.AppendFormat("<div class='empty'>No project loaded.</div>");
        }

        private static void RenderProgramNode(Program program, StringBuilder sb)
        {
            sb.AppendFormat("<div class='program' data-name='{0}'>", program.Name);
            sb.AppendLine();
            sb.AppendFormat("<span class='name'>{0}</span>", program.Name);
            foreach (var segment in program.SegmentMap.Segments.Values)
            {
                RenderSegmentNode(program, segment, sb);
            }
            sb.AppendLine("</div>");
        }
    }
}
