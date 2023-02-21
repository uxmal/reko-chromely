using NUnit.Framework;
using Reko.Core;
using System;
using System.Linq;
using Reko.Chromely.Renderers;
using Reko.Core.Memory;
using System.ComponentModel.Design;
using Reko.Arch.X86;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Reko.Chromely.UnitTests
{
	[TestFixture]
	public class HexViewRendererTests
	{
		private Program program;

		private void Given_Program() {
			var sc = new ServiceContainer();
			var options = new Dictionary<string, object>();
			var cpu = new X86ArchitectureFlat32(sc, "x86-protected-32", options);
			var platform = new Environments.Windows.Win32Platform(sc, cpu);

			this.program = new Program()
			{
				Platform = platform
			};
        }

		private void Given_Data(uint uAddress, byte[] bytes) {
			var mem = new ByteMemoryArea(Address.Ptr32(uAddress), bytes);
			var seg = new ImageSegment("dummy", mem, AccessMode.ReadWriteExecute);
			var map = new SegmentMap(mem.BaseAddress, seg);
			this.program.SegmentMap = map;
			this.program.BuildImageMap();
		}

		private void AssertEqual(string sExp, string sActual) {
			if(sExp != sActual) {
				Console.WriteLine(sActual);
				Assert.AreEqual(sExp, sActual);
			}
		}

		[Test]
		public void Hv_SingleLine() {
			Given_Program();
			Given_Data(0x1000, Enumerable.Range(0, 16).Select(i => (byte)i).ToArray());
			var hr = new HexViewRenderer(program, program.SegmentMap.BaseAddress, 16);
			var sActual = hr.Render();

			var sExp =
			#region expected
				@"[
{
""addr"": ""00001000"",""addrLabel"": ""0x00001000"",""hex"": [
{""t"": ""d"",""d"": ""00""}
,{""t"": ""d"",""d"": ""01""}
,{""t"": ""d"",""d"": ""02""}
,{""t"": ""d"",""d"": ""03""}
,{""t"": ""d"",""d"": ""04""}
,{""t"": ""d"",""d"": ""05""}
,{""t"": ""d"",""d"": ""06""}
,{""t"": ""d"",""d"": ""07""}
,{""t"": ""d"",""d"": ""08""}
,{""t"": ""d"",""d"": ""09""}
,{""t"": ""d"",""d"": ""0A""}
,{""t"": ""d"",""d"": ""0B""}
,{""t"": ""d"",""d"": ""0C""}
,{""t"": ""d"",""d"": ""0D""}
,{""t"": ""d"",""d"": ""0E""}
,{""t"": ""d"",""d"": ""0F""}
]
}]
";
			#endregion

			AssertEqual(sExp, sActual);
		}

		[Test]
		public void Hv_SingleLine_Misaligned()
		{
			Given_Program();
			Given_Data(0x1000, Enumerable.Range(0, 16).Select(i => (byte)i).ToArray());
			var hr = new HexViewRenderer(program, program.SegmentMap.BaseAddress + 1, 16);
			var sActual = hr.Render();

			var sExp =
			#region expected
				@"[
{
""addr"": ""00001001"",""addrLabel"": ""0x00001001"",""hex"": [
{""t"": ""d"",""d"": """"}
,{""t"": ""d"",""d"": ""01""}
,{""t"": ""d"",""d"": ""02""}
,{""t"": ""d"",""d"": ""03""}
,{""t"": ""d"",""d"": ""04""}
,{""t"": ""d"",""d"": ""05""}
,{""t"": ""d"",""d"": ""06""}
,{""t"": ""d"",""d"": ""07""}
,{""t"": ""d"",""d"": ""08""}
,{""t"": ""d"",""d"": ""09""}
,{""t"": ""d"",""d"": ""0A""}
,{""t"": ""d"",""d"": ""0B""}
,{""t"": ""d"",""d"": ""0C""}
,{""t"": ""d"",""d"": ""0D""}
,{""t"": ""d"",""d"": ""0E""}
,{""t"": ""d"",""d"": ""0F""}
]
}]
";
			#endregion

			AssertEqual(sExp, sActual);
		}
	}
}
