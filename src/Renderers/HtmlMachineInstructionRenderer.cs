using Reko.Core;
using Reko.Core.Machine;
using System.IO;

namespace Reko.Chromely.Renderers
{
    /// <summary>
    /// This class renders <see cref="MachineInstruction"/>s as HTML.
    /// </summary>
    internal class HtmlMachineInstructionRenderer : MachineInstructionRenderer
    {
        private readonly TextWriter w;
        private Address? addrInstr;

        public HtmlMachineInstructionRenderer(TextWriter textWriter)
        {
            this.w = textWriter;
        }

        public void BeginInstruction(Address addr)
        {
            this.addrInstr = addr;
            w.Write("<div class='instr'>");
        }

        public void EndInstruction()
        {
            w.Write("</div>");
        }


        public void BeginOperand()
        {
        }

        public void EndOperand()
        {
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            w.Write("<span class='addr' data-addr='{0}'>{1}</span>", addr, formattedAddress);
        }

        public void WriteAddress(string formattedAddress, ulong uAddr)
        {
            w.Write("<span class='addr' data-addr='{0:X}'>{1}</span>", uAddr, formattedAddress);
        }

        public void WriteFormat(string fmt, params object[] parms)
        {
            w.Write(fmt, parms);
        }

        public Address Address => addrInstr!;

        public void AddAnnotation(string a)
        {
            w.Write("<span class='anno'>");
            w.Write("; {0}", a);
            w.Write("</span>");
        }

        public void WriteMnemonic(string sMnemonic)
        {
            w.Write("<span class='mnem'>{0}</span>", sMnemonic);
        }


        public void Tab()
        {
        }

        public void WriteString(string s)
        {
            w.Write(s);
        }

        public void WriteChar(char c)
        {
            w.Write(c);
        }

        public void WriteUInt32(uint n)
        {
            w.Write(n);
        }
    }
}