using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
    class Nyom
    {
        static string elválasztójel = "[green]{/}";
        public string szöveg;

        public Nyom(string s)
        {
            szöveg = s+elválasztójel;
        }

        public Nyom(Nyom ny, string s)
        {
            szöveg = ny.szöveg + s + elválasztójel;
        }

        public static Nyom operator /(Nyom ág, string levél)
        {
            return new Nyom(ág, levél);
        }
    }
}
