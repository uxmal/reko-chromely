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
