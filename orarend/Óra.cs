using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{
		public class Óra
		{
			public static List<Óra> lista = new List<Óra>();
			public static Dictionary<string, string> nap2int;

			public Tanár tanár;
			public Tantárgy tantárgy;
			public Csoport csoport;
			public int nap;
			public int hanyadik;
			public Terem terem;
			public bool UtolsoTerem = false;

			public static void Init(Nyom nyom)
			{
				nap2int = DB_Beolvas("DB_napok.tsv", 0, 1, nyom / "DB_Beolvas:nap2int");
			}

			public Óra(string[] sortömb, Nyom nyom)
			{
				tanár = Tanár.lista.First(t => t.kr == sortömb[0]);
				tantárgy = new Tantárgy(sortömb, nyom);
				csoport = new Csoport(sortömb, nyom);
				nap = int.Parse(nap2int[sortömb[3]]);
				hanyadik = int.Parse(sortömb[4]);
				terem = Terem.lista.First(t => t.kr == sortömb[5]);

				lista.Add(this);

				DebugConsole.Write($"[red]{{({lista.Count})}}");
			}

            public override string ToString()
            {
                return $"([blue]{{{nap}}};[blue]{{{hanyadik}}}) {csoport.sávID} ([green]{{{tanár.mg}}};[blue]{{{tantárgy.név}}}) ({terem.st})";
            }
        }
	}
}

