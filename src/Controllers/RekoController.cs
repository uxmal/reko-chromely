#region
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
using Chromely.Core.Network;
using Reko.Arch.X86;
using Reko.Chromely.Renderers;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reko.Chromely.Controllers
{
    [ControllerProperty(Name = nameof(RekoController))]
    public class RekoController : ChromelyController
    {
        private IChromelyConfiguration config;
        private Task? bgTask;

        public RekoController(IChromelyConfiguration config)
        {
            this.config = config;
        }

        [RequestAction(Name="Execute", RouteKey ="/executejavascript/execute")]
        public IChromelyResponse Execute(IChromelyRequest request)
        {
            if (bgTask != null && !bgTask.IsCompleted)
            return new ChromelyResponse(request.Id)
            {
                StatusText = "OK",
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = (int)System.Net.HttpStatusCode.OK,
                Data = "Thread busy"
            };
            this.bgTask = Task.Run(ScanBinary);
            var response = new ChromelyResponse(request.Id)
            {
                 StatusText= "OK",
                ReadyState = (int)ReadyState.ResponseIsReady,
                Status = (int)System.Net.HttpStatusCode.OK,
                Data = "Thread Started"
            };
            return response;
        }

        private void ScanBinary()
        {
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
            foreach (var instr in dasm)
            {
                instr.Render(renderer, options);
            }
            var sDasm = sw.ToString();
            var js = $"OnScanComplete(\"{sDasm}\")";
            SendJavascriptToClient(js);
        }

        private void SendJavascriptToClient(string v)
        {
            config.JavaScriptExecutor.ExecuteScript(v);
        }
    }
}
