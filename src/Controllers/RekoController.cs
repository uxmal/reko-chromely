using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Network;
using Reko.Arch.X86;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
            var sDasm = string.Join("<br />\\r\\n", dasm);
            var js = $"OnScanComplete(\"{sDasm}\")";
            SendJavascriptToClient(js);
        }

        private void SendJavascriptToClient(string v)
        {
            config.JavaScriptExecutor.ExecuteScript(v);
        }
    }
}
