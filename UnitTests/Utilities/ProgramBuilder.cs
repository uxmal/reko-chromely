﻿using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.Chromely.UnitTests.Utilities
{
    /// <summary>
    /// Supports building a intermediate code program directly without having
    /// to first generate machine code and scanning it.
    /// </summary>
    public class ProgramBuilder
    {
        private uint procCount;
        private Dictionary<string, Procedure> nameToProcedure = new Dictionary<string, Procedure>();
        private Dictionary<string, Block> blocks = new Dictionary<string, Block>();
        private List<ProcUpdater> unresolvedProcedures = new List<ProcUpdater>();

        public ProgramBuilder() : this(new FakeArchitecture(new ServiceContainer()))
        {
        }

        public ProgramBuilder(IProcessorArchitecture arch)
        {
            Program = new Program
            {
                Architecture = arch,
            };
        }

        public ProgramBuilder(ByteMemoryArea mem)
        {
            Program = new Program
            {
                SegmentMap = new SegmentMap(
                    mem.BaseAddress,
                    new ImageSegment("code", mem, AccessMode.ReadWriteExecute)),
                Architecture = new FakeArchitecture(new ServiceContainer())
            };
        }

        public Program Program { get; set; }

        public void Add(Procedure proc, UserProcedure userProc = null)
        {
            ++procCount;
            var addr = Address.Ptr32(procCount * 0x1000u);
            Program.Procedures[addr] = proc;
            Program.CallGraph.AddProcedure(proc);
            nameToProcedure[GuessName(userProc, proc)] = proc;
            if (userProc != null)
            {
                Program.User.Procedures[addr] = userProc;
            }
        }

        private string GuessName(UserProcedure userProc, Procedure proc = null)
        {
            if (userProc != null)
            {
                if (!string.IsNullOrEmpty(userProc.Name))
                    return userProc.Name;
                if (!string.IsNullOrEmpty(userProc.CSignature))
                {
                    int i = userProc.CSignature.IndexOf('(');
                    if (i > 0)
                    {
                        var name = userProc.CSignature.Remove(i);
                        do
                        {
                            --i;
                        } while (i > 0 && (char.IsLetterOrDigit(name[i]) || name[i] == '_'));
                        return name.Substring(i + 1);
                    }
                }
            }
            if (proc != null)
            {
                return proc.Name;
            }
            else
            {
                return null;
            }
        }

        public void Add(ProcedureBuilder mock)
        {
            mock.ProgramBuilder = this;
            Add(mock.Procedure, null);
            unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
        }

        public Procedure Add(string procName, Action<ProcedureBuilder> testCodeBuilder)
        {
            return Add(
                new UserProcedure(
                    Address.FromConstant(Constant.Zero(Program.Architecture.WordWidth)),
                    procName
                ),
                testCodeBuilder
            );
        }

        public Procedure Add(UserProcedure userProc, Action<ProcedureBuilder> testCodeBuilder)
        {
            var mock = new ProcedureBuilder(Program.Architecture, GuessName(userProc));
            mock.ProgramBuilder = this;
            mock.LinearAddress = (uint)((procCount + 1) * 0x1000);
            testCodeBuilder(mock);
            Add(mock.Procedure, userProc);
            unresolvedProcedures.AddRange(mock.UnresolvedProcedures);
            return mock.Procedure;
        }

        public void BuildCallgraph()
        {
            foreach (Procedure proc in Program.Procedures.Values)
            {
                foreach (Statement stm in proc.Statements)
                {
                    var call = stm.Instruction as CallInstruction;
                    if (call == null)
                        continue;
                    var pc = call.Callee as ProcedureConstant;
                    if (pc == null)
                        continue;
                    var callee = pc.Procedure as Procedure;
                    if (callee == null)
                        continue;
                    Program.CallGraph.AddEdge(stm, callee);
                }
            }
        }

        public Program BuildProgram()
        {
            return BuildProgram(Program.Architecture);
        }

        public Program BuildProgram(IProcessorArchitecture arch)
        {
            Program.Architecture = arch;
            ResolveUnresolved();
            BuildCallgraph();
            if (Program.SegmentMap == null)
                Program.SegmentMap = new SegmentMap(Address.Ptr16(0x1000));
            Program.SegmentMap.AddSegment(
                new ByteMemoryArea(Address.Ptr32(0x1000), new byte[Program.Procedures.Count * 0x1000]),
                ".text", AccessMode.Execute);

            Program.Platform = new DefaultPlatform(null, arch);
            return Program;
        }

        public void ResolveUnresolved()
        {
            foreach (ProcUpdater pcu in unresolvedProcedures)
            {
                Procedure proc;
                if (!nameToProcedure.TryGetValue(pcu.Name, out proc))
                    throw new ApplicationException("Unresolved procedure name: " + pcu.Name);
                pcu.Update(proc);
            }
            unresolvedProcedures.Clear();
        }
    }

    public abstract class ProcUpdater
    {
        private readonly string name;
        public ProcUpdater(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public abstract void Update(Procedure proc);
    }

    public class ProcedureConstantUpdater : ProcUpdater
    {
        private CallInstruction ci;

        public ProcedureConstantUpdater(string name, CallInstruction ci) : base(name)
        {
            this.ci = ci;
        }

        public override void Update(Procedure proc)
        {
            ci.Callee = new ProcedureConstant(PrimitiveType.Word32, proc);
        }

    }

    public class ApplicationUpdater : ProcUpdater
    {
        private Application appl;

        public ApplicationUpdater(string name, Application appl) : base(name)
        {
            this.appl = appl;
        }

        public override void Update(Procedure proc)
        {
            appl.Procedure = new ProcedureConstant(PrimitiveType.Ptr32, proc);
        }
    }
}

