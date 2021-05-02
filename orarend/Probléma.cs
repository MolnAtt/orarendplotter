using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
    partial class Program
    {
        class Probléma
        {
            string szöveg;

            public static List<Probléma> lista = new List<Probléma>();

            public Probléma(Nyom nyom, string s)
            {
                this.szöveg = nyom.szöveg+s;
                lista.Add(this);
                DebugConsole.WriteLine(szöveg);
            }

            public override string ToString() => szöveg;
            public static void Összes_Kiírása()
            {
                foreach (Probléma probléma in lista)
                {
                    DebugConsole.WriteLine(probléma.ToString());
                }
            }

        }
    }
}
