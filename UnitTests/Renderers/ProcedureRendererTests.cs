#region License
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

using NUnit.Framework;
using Reko.Chromely.Renderers;
using Reko.Chromely.UnitTests.Utilities;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Reko.Chromely.UnitTests.Renderers
{
    [TestFixture]
    public class ProcedureRendererTests
    {
        private void RunIrTest(string testName, string sExp, Action<ProcedureBuilder> builder)
        {
            var m = new ProcedureBuilder(testName);
            builder(m);
            var program = CreateProgram(m.Procedure);
            var sActual = ProcedureRenderer.RenderProcedureIR(program, m.Procedure.EntryAddress.ToString());
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private void RunHllTest(string testName, string sExp, Action<Procedure, AbsynCodeEmitter> builder)
        {
            var arch = new FakeArchitecture(new ServiceContainer());
            var frame = arch.CreateFrame();
            var proc = Procedure.Create(arch, Address.Ptr32(0x00123400), frame);
            proc.Body = new List<Core.Absyn.AbsynStatement>();
            var m = new AbsynCodeEmitter(proc.Body);
            builder(proc, m);
            var program = CreateProgram(proc);
            var sActual = ProcedureRenderer.RenderProcedureHll(program, proc.EntryAddress.ToString());
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        private static Program CreateProgram(Procedure proc)
        {
            return new Program
            {
                Architecture = proc.Architecture,
                Procedures = {
                    { proc.EntryAddress,proc }
                }
            };
        }

        [Test]
        public void ProcRnd_IR_QuoteString()
        {
            var sExp =
            #region Expected
@"ProcRnd_IR_QuoteString_entry:<br />
l1:<br />
	Mem0[0x00123400:(str char)] = &quot;&lt;Up &amp; down&gt;&quot;<br />
ProcRnd_IR_QuoteString_exit:<br />
";
            #endregion

            RunIrTest(nameof(ProcRnd_IR_QuoteString), sExp,  m =>
            {
                m.MStore(m.Word32(0x00123400), Constant.String("<Up & down>", StringType.NullTerminated(PrimitiveType.Char)));
            });
        }

        [Test]
        public void ProcRnd_HLL_Signature()
        {
            var sExp =
            #region Expected
@"<span class='type' data-typename='int32'>int32</span> fn00123400(BYTE * arg1, BYTE * arg2)<br />
{<br />
	<span class='kw'>if</span> (arg1 != 0x00)<br />
	{<br />
		*arg1 = 0x00;<br />
		arg2 = &quot;&lt;Up &amp; down&gt;&quot;;<br />
	}<br />
}<br />
";
            #endregion
            RunHllTest(nameof(ProcRnd_HLL_Signature), sExp, (proc, m) =>
            {
                var arg1 = new Identifier(
                    "arg1",
                    new Pointer(new TypeReference("BYTE", PrimitiveType.Byte), 32),
                    new TemporaryStorage("t1", 3, PrimitiveType.Word32));
                var arg2 = new Identifier(
                    "arg2",
                    new Pointer(new TypeReference("BYTE", PrimitiveType.Byte), 32),
                    new TemporaryStorage("t2", 4, PrimitiveType.Word32));


                proc.Signature = FunctionType.Func(
                    new Identifier("", PrimitiveType.Int32, new TemporaryStorage("", 0, PrimitiveType.Word32)),
                    arg1,
                    arg2);
                m.If(m.Ne(arg1, Constant.Zero(PrimitiveType.Word32)), t =>
                {
                    t.Assign(t.Deref(arg1), m.Byte(0));
                    t.Assign(arg2, Constant.String("<Up & down>", StringType.NullTerminated(PrimitiveType.Char)));
                });
            });
        }
    }
}
