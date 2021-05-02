using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{

		public class Osztály
		{
			public static List<Osztály> lista = new List<Osztály>();
			public int sorszám;
			public string évfolyam; // string, mert NY.F, NY.E, KNY.A	
			public string szekció;
			public string név;
			public int maxtagoltság; // a nagy diákórarendben a lehetséges legtöbb egyszerre tartott órája az osztálynak (pl. ha 11 csoportra bontják valamikor, akkor akár 11!)

			public Osztály(string[] sortömb, Nyom nyom)
			{
				sorszám = int.Parse(sortömb[0]);
				név = sortömb[3];
				évfolyam = sortömb[1].ToUpper(); // Az azonosításhoz mindenképpen nagybetűsítünk! ("NY", "KNY")
				szekció = sortömb[2].ToUpper(); // Az azonosításhoz mindenképpen nagybetűsítünk! ("A", "F", ...)

				lista.Add(this);

				DebugConsole.Write($"[blue]{{({lista.Count})}}");

			}

			public override string ToString() => $"({sorszám}) {név} ({évfolyam}.{szekció})";

            /// <summary>
            /// Az osztály neve alapján visszaadja az osztályt magát.
            /// </summary>
            /// <param name="neve">Az osztály neve, pl. "11.f" vagy "nyf"</param>
            /// <returns></returns>
            public static Osztály Get(string neve) => lista.First(x => x.név == neve);

			/// <summary>
			/// Az osztály sorszáma alapján visszaadja az osztályt magát. Hibakezel.
			/// </summary>
			/// <param name="neve">Az osztály neve, pl. "11.f" vagy "nyf"</param>
			/// <returns></returns>
			public static bool TryGet(int sorszám, Nyom nyom, out Osztály result)
			{
				DebugConsole.WriteLine(nyom, $"Keresem: {sorszám}");

				int ix = Osztály.lista.FindIndex(x => x.sorszám == sorszám);
				result = ix >= 0? Osztály.lista[ix]:null;
				return ix >= 0;
			}

			/// <summary>
			/// Az osztály évfolyama és szekciója alapján visszaadja az osztályt magát.
			/// </summary>
			/// <param name="neve">Az osztály neve, pl. "11.f" vagy "nyf"</param>
			/// <returns></returns>
			public static bool TryGet(string évf, string szek, Nyom nyom, out Osztály result)
			{
				DebugConsole.WriteLine(nyom, $"Keresem: { évf} { szek}");
				int ix = Osztály.lista.FindIndex(x => x.évfolyam == évf && x.szekció == szek);
				result = ix >= 0 ? Osztály.lista[ix] : null;
				return ix>=0;
			}
		}
	}
}
