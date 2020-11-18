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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Reko.Chromely.Renderers
{
    public class ProcedureListRenderer
    {
        private readonly IDecompiler decompiler;
        public ProcedureListRenderer(IDecompiler decompiler)
        {
            this.decompiler = decompiler;
        }

        public string Render(string filter)
        {
            var project = decompiler.Project;
            if (project is null)
                return "[]";
            var viewModel = project.Programs
                .Select(program => program.Procedures.Values
                    .Select(proc => new ProcedureListItem
                    {
                        sProgram = program.Filename,
                        sAddress = proc.EntryAddress.ToString(),
                        name = proc.Name
                    }))
                .SelectMany(pp => pp);
            if (!string.IsNullOrEmpty(filter))
            {
                viewModel = viewModel.Where(item => ItemMatchesFilter(item, filter));
            }
            return JsonSerializer.Serialize(viewModel);
        }

        private static bool ItemMatchesFilter(ProcedureListItem item, string filter)
        {
            return
                item.sAddress != null && item.sAddress.Contains(filter,StringComparison.InvariantCultureIgnoreCase) ||
                item.name != null && item.name.Contains(filter, StringComparison.InvariantCultureIgnoreCase);
        }

        public class ProcedureListItem
        {
            public string? sProgram { get; set; }
            public string? sAddress { get; set; }
            public string? name { get; set; }
        }
    }
}
