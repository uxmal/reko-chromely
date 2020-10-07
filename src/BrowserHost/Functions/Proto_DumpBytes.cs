using Microsoft.VisualBasic.CompilerServices;
using Reko.Chromely.Renderers;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Chromely.BrowserHost.Functions
{
    public class Proto_DumpBytes
    {
        private readonly IDecompiler decompiler;

        public Proto_DumpBytes(IDecompiler decompiler)
        {
            this.decompiler = decompiler;
        }

        public string Execute(string sProgramName, string sAddress, long length)
        {
            var program = decompiler.Project?.Programs.FirstOrDefault(p => p.Name == sProgramName);
            if(program == null || !program.Platform.TryParseAddress(sAddress, out var addr))
            {
                return "[]";
            }
            var hv = new HexViewRenderer(program, addr, length);
            return hv.Render();
        }
    }
}
