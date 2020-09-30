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
    /// <summary>
    /// Background task that executes asynchronous operations
    /// </summary>
	public class ExecuteJavascriptTask : CefTask
	{
        private readonly CefV8Context ctx;

        private readonly CefV8Value onScanComplete;

        public ExecuteJavascriptTask(CefV8Context context) {
            ctx = context;

            ctx.Enter();
            var glbl = ctx.GetGlobal();
            onScanComplete = glbl.GetValue("OnScanComplete");
            ctx.Exit();
		}

        /// <summary>
        /// Send the resulting disassembly to the browser
        /// by calling a JS function
        /// </summary>
        /// <param name="dasm"></param>
        private void SendToClient(string dasm) {
            ctx.Enter();

            // invoke JS function
            var argString = CefV8Value.CreateString(dasm);
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

    /// <summary>
    /// Synchronous handler for the function call
    /// </summary>
	public class ExecuteJavascript : CefV8Handler
	{
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception) {
            returnValue = CefV8Value.CreateNull();
            exception = null;

            /**
             * this handler is synchronous, so we create an asynchronous task on the render thread.
             * the task is given the current execution context
             * so that the task can call-back into JS code when it wants to
             **/
            var task = new ExecuteJavascriptTask(CefV8Context.GetCurrentContext());
            CefTaskRunner.GetForCurrentThread().PostTask(task);

            return true;
        }
	}
}
