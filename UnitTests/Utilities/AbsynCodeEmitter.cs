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

using System;
using System.Collections.Generic;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Types;

namespace Reko.Chromely.UnitTests.Renderers
{
    internal class AbsynCodeEmitter : ExpressionEmitter
    {
        private List<AbsynStatement> stmts;

        public AbsynCodeEmitter(List<AbsynStatement> stmts)
        {
            this.stmts = stmts;
        }

        public AbsynAssignment Assign(Expression dst, Expression src)
        {
            var ass = new AbsynAssignment(dst, src);
            stmts.Add(ass);
            return ass;
        }

        public void Declare(Identifier id, Expression initializer = null)
        {
            var decl = new AbsynDeclaration(id, initializer);
            stmts.Add(decl);
        }

        public void DoWhile(Action<AbsynCodeEmitter> bodyGen, BinaryExpression cond)
        {
            var bodyStmts = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(bodyStmts);
            bodyGen(m);
            stmts.Add(new AbsynDoWhile(bodyStmts, cond));
        }

        public void For(
            Func<AbsynCodeEmitter, AbsynAssignment> init,
            Func<AbsynCodeEmitter, Expression> cond,
            Func<AbsynCodeEmitter, AbsynAssignment> update,
            Action<AbsynCodeEmitter> body)
        {
            var initStms = new List<AbsynStatement>();
            var initEmitter = new AbsynCodeEmitter(initStms);
            var initStm = init(initEmitter);
            var condExp = cond(initEmitter);
            var updateStm = update(initEmitter);
            var bodyStms = new List<AbsynStatement>();
            var bodyEmitter = new AbsynCodeEmitter(bodyStms);
            body(bodyEmitter);
            this.stmts.Add(new AbsynFor(initStm, condExp, updateStm, bodyStms));
        }

        public void If(Expression id, Action<AbsynCodeEmitter> then)
        {
            var thenStmts = new List<AbsynStatement>();
            var thenEmitter = new AbsynCodeEmitter(thenStmts);
            then(thenEmitter);
            stmts.Add(new AbsynIf(id, thenStmts));
        }

        public void If(Expression id, Action<AbsynCodeEmitter> then, Action<AbsynCodeEmitter> els)
        {
            var thenStmts = new List<AbsynStatement>();
            var thenEmitter = new AbsynCodeEmitter(thenStmts);
            then(thenEmitter);
            var elseStmts = new List<AbsynStatement>();
            var elseEmitter = new AbsynCodeEmitter(elseStmts);
            els(elseEmitter);
            stmts.Add(new AbsynIf(id, thenStmts, elseStmts));
        }

        public void Return(Expression expr = null)
        {
            var ret = new AbsynReturn(expr);
            stmts.Add(ret);
        }

        public void SideEffect(Expression e)
        {
            var s = new AbsynSideEffect(e);
            stmts.Add(s);
        }

        public void While(Expression cond, Action<AbsynCodeEmitter> bodyGen)
        {
            var bodyStmts = new List<AbsynStatement>();
            var m = new AbsynCodeEmitter(bodyStmts);
            bodyGen(m);
            stmts.Add(new AbsynWhile(cond, bodyStmts));
        }

        /// <summary>
        /// Generates a cast expression which coerces the <paramref name="expr"/> to
        /// the data type <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">Type to coerce to.</param>
        /// <param name="expr">Value to coerce.</param>
        /// <returns>A cast expression.</returns>
        /// <remarks>
        /// This method is not on <see cref="ExpressionEmitter"/> because we want to 
        /// discourage the use of <see cref="Cast"/> expressions in early stages of Reko.
        /// Use <see cref="Slice"/> or <see cref="Convert"/> expressions instead.
        /// </remarks>
        public Cast Cast(DataType dataType, Expression expr)
        {
            return new Cast(dataType, expr);
        }
    }
}