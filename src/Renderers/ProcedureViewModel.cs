using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Chromely.Renderers
{
    public class ProcedureViewModel
    {
        private int i;

        public ProcedureViewModel(int i)
        {

            this.i = i;
            var rnd = new Random(i);
            int Rnd(int mod) => rnd.Next() % mod;
            var sb = new StringBuilder();
            int lines = Rnd(6) + Rnd(6) + Rnd(6) + 1;
            for (int iline = 0; iline < lines; ++iline)
            {
                int linelength = (rnd.Next() % 15) + (rnd.Next() % 15) + (rnd.Next() % 15);
                for (int j = 0; j < linelength; ++j)
                {
                    sb.Append((char)((rnd.Next() % 40) + 'A'));
                }
                sb.AppendLine();
            }
            this.Body = sb.ToString();
        }

        public string Name => $"proc{i:X8}";

        public string Body { get; }
    }
}
