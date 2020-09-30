using Reko.Arch.X86;
using Reko.Chromely.Renderers;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using Xilium.CefGlue;

namespace Reko.Chromely.BrowserHost.Functions
{
	public class ExecuteJavascriptTask : CefTask
	{
        private readonly CefV8Context ctx;
        public ExecuteJavascriptTask(CefV8Context context) {
            ctx = context;
		}

        private void SendToClient(string dasm) {
            ctx.Enter();
            var glbl = ctx.GetGlobal();

            var argString = CefV8Value.CreateString(dasm);

            var onScanComplete = glbl.GetValue("OnScanComplete");
            onScanComplete.ExecuteFunction(null, new CefV8Value[] { argString });

            ctx.Exit();
        }

        protected override void Execute() {
            var rnd = new Random();
            var buf = new byte[100];
            rnd.NextBytes(buf);
            var mem = new MemoryArea(Address.Ptr32(0x00123400), buf);
            var arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32");
            var rdr = arch.Endianness.CreateImageReader(mem, mem.BaseAddress);
            var dasm = arch.CreateDisassembler(rdr);
            var sw = new StringWriter();
            var renderer = new HtmlMachineInstructionRenderer(sw);
            var options = new MachineInstructionRendererOptions();
            foreach (var instr in dasm) {
                instr.Render(renderer, options);
            }
            var sDasm = sw.ToString();
            SendToClient(sDasm);
        }
	}

	public class ExecuteJavascript : CefV8Handler
	{
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception) {
            returnValue = CefV8Value.CreateNull();
            exception = null;

            var task = new ExecuteJavascriptTask(CefV8Context.GetCurrentContext());
            CefTaskRunner.GetForCurrentThread().PostTask(task);

            return true;
        }
	}
}
