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
            var addrLineStart = address.Align(LineSize);
            var sep = "";
            for(long o=offset; o<length; o += LineSize)
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

        private void RenderLine(ImageSegment segment, Address address, long o)
        {
            sb.AppendLine("{");
            var lineAddr = address + o;
            var lineAddrString = lineAddr.ToString();
            sb.AppendLine($"\"addr\": \"{lineAddrString}\",");
            sb.AppendLine($"\"addrLabel\": \"0x{lineAddrString}\",");
            sb.AppendLine("\"hex\": [");
            var sep = "";
            for(int i=0; i<LineSize; i++)
            {
                sb.Append(sep);
                sep = ",";
                sb.Append("{");
                sb.AppendFormat("\"t\": \"{0}\",", GetTypeForAddress(lineAddr));
                sb.AppendFormat("\"d\": \"{0:X2}\"", segment.MemoryArea.Bytes[o + i]);
                sb.AppendLine("}");
            }
            sb.AppendLine("]");
            sb.Append("}");
        }
    }
}
