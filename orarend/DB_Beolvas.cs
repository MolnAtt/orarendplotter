using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
    partial class Program
    {
		/// <summary>
		/// Beolvassa az n oszlopos "fajlnev" nevű fájlt és létrehoz az alapján egy szótárt amelynek kulcsait a táblázat "keyCol"-ik oszlop, értékeit pedig a "keyValue"-ik oszlop szolgáltatja.
		/// </summary>
		/// <param name="fajlnev">A fájl, amiben az adatbázis van.</param>
		/// <param name="keyCol">A kulcsokat tartalmazó oszlop sorszáma.</param>
		/// <param name="valueCol">Az értékeket tartalmzó oszlop sorszáma.</param>
		/// <returns>Az a Dictionary, amiben csak ez a két oszlop szerepel beolvasva.</returns>
		public static Dictionary<string, string> DB_Beolvas(string fajlnev, int keyCol, int valueCol, Nyom nyom)
		{
			DebugConsole.WriteLine($"DB_Beolvas({ fajlnev},{ keyCol},{ valueCol}):");

			Dictionary<string, string> db = new Dictionary<string, string>();
            foreach (string[] sortömb in File.ReadAllLines(fajlnev, Encoding.UTF8).Select(sor => sor.Split('\t')))
            {
				db.Add(sortömb[keyCol], sortömb[valueCol]);
			}
			return db;
		}
	}
}
