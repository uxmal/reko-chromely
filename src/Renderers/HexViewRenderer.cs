using Reko.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Chromely.Renderers
{
    using Program = Reko.Core.Program;

    public class HexViewRenderer
    {
        private readonly Program program;
        private readonly Address address;
        private readonly long length;

        private readonly StringBuilder sb;

        private const int LineSize = 16;

        public HexViewRenderer(Program program, Address address, long length)
        {
            this.program = program;
            this.address = address;
            this.length = length;
            this.sb = new StringBuilder();
        }

        public string Render()
        {
            sb.AppendLine("[");
            if(program.SegmentMap.TryFindSegment(address, out var segment))
            {
                var offset = address - segment.MemoryArea.BaseAddress;
                var length = Math.Min(this.length, segment.MemoryArea.Length - offset);
                RenderLines(segment, this.address, offset, length);
            }
            sb.AppendLine("]");
            return sb.ToString();
        }

        private void RenderLines(ImageSegment segment, Address address, long offset, long length)
        {
            var sep = "";
            for(long o=offset; o<offset + length; o += LineSize)
            {
                sb.Append(sep);
                sep = ",";
                RenderLine(segment, address, o);
            }
        }

        private string GetTypeForAddress(Address address)
        {
            if(!program.ImageMap.TryFindItem(address, out var item))
            {
                // undefined
                return "u";
            }

            if(item is ImageMapBlock)
            {
                // code
                return "c";
            }

            // data
            return "d";
        }

        private IEnumerable<string> RenderBytes(ImageSegment segment, long o)
        {
            for (int i = 0; i < LineSize; i++)
            {
                yield return segment.MemoryArea.Bytes[o + i].ToString("X2");
            }
        }

        private void RenderLine(ImageSegment segment, Address address, long o)
        {
            sb.AppendLine("{");
            var lineAddr = address + o;
            var lineAddrString = lineAddr.ToString();

            RenderKeyValue("addr", lineAddrString, ",");
            RenderKeyValue("addrLabel", "0x" + lineAddrString, ",");
            RenderKeyValue("hex", () =>
            {
                RenderArray(RenderBytes(segment, o), (b) =>
                {
                    sb.Append("{");
                    RenderKeyValue("t", GetTypeForAddress(lineAddr), ",");
                    RenderKeyValue("d", b, "");
                    sb.AppendLine("}");
                });
            }, "");
            sb.Append("}");
        }

        private void RenderArray(IEnumerable<string> elements, Action<string> elementRenderer)
        {
            var sep = "";
            sb.AppendLine("[");
            foreach (var obj in elements)
            {
                sb.Append(sep);
                sep = ",";
                elementRenderer(obj);
            }
            sb.AppendLine("]");
        }

        private void RenderKeyValue(string key, Action valueRenderer, string sep)
        {
            sb.AppendFormat("\"{0}\": ", key);
            valueRenderer();
            sb.Append(sep);
        }

        private void RenderKeyValue(string key, string value, string sep)
        {
            sb.AppendFormat("\"{0}\": \"{1}\"{2}", key, value, sep);
        }
    }
}
