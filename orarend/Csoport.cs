using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{
		public class Csoport
		{
			public static bool sávbontás_részletezése = false;
			public static Dictionary<string, string> customsávkódok;

			public string krétakód;
			public string sávID;
			public string csopID;
			public List<Osztály> osztályok = new List<Osztály>();

			public Csoport(string[] sor, Nyom nyom)
			{
				sávID = sor[2];
				csopID = sor[6];
				osztályok = str2oszt(sor[2], nyom);
				krétakód = sor[7];
			}
			string GetOsztString()
			{
				string sum = "[" + osztályok.First().név;
                foreach (Osztály o in osztályok.Skip(1))
                {
					sum += "," + o.név;
                }
				return sum + "]";
			}
			public override string ToString() => $"{sávID} {csopID} {GetOsztString()} {krétakód}";

			/// <summary>
			/// Az osztálykódokból álló listákat az osztályok listáinak elemeiből álló osztálylistává alakítja. 
			/// </summary>
			/// <param name="kód"></param>
			/// <returns></returns>
			public static List<Osztály> str2oszt(string kód, Nyom nyom) // a kód egy ilyen : "9.abd,10.ef,11.bcd,12.aef"
			{
				DebugConsole.WriteLine(nyom, "indul");

				List<Osztály> osztálylista = new List<Osztály>();

				customsávkódok = DB_Beolvas("DB_customsávkódok.tsv", 0, 1, nyom / "DB_beolvas:customsávkódok");

				string[] évfolyamonként = kód.Split(','); // évfolyamonként: [ "9.abd"  ,  "10.ef"  ,  "11.bcd"  ,  "12.aef" ]
				foreach (string évfolyamrész in évfolyamonként) // egy évfolyamrész: "11.bcd"
				{
					string[] évfolyamsplit = évfolyamrész.Split('.');
					if (évfolyamsplit.Length == 1) // ez csak a kny, nye, nyf, oktv, stb. esetében lehet, szóval ez egy custom sávnév kell legyen.
						osztálylista.AddRange(str2oszt(customsávkódok[évfolyamsplit[0]], nyom)); // csak akkor lehet végtelen kör, ha az adatbázisban kör van!
					else
					{
						string évf = évfolyamsplit[0];
						foreach (char betű in évfolyamsplit[1])
						{
							Osztály osztály;
							if (!Osztály.TryGet(évf.ToUpper(), betű.ToString().ToUpper(), nyom / "osztály.TryGet", out osztály))
								new Probléma(nyom, $"[red]{{BAJ VAN!}} nem találtam meg az osztályt az évfolyam és szekció alapján. ");
							osztálylista.Add(osztály);
						}
					}
				}

				if (sávbontás_részletezése)
				{
					DebugConsole.WriteLine(nyom, $"A \"{kód}\" kódból a következő osztályokat fejtettem vissza:");
					foreach (var item in osztálylista)
						DebugConsole.WriteLine(nyom, $"évfolyam: {item.évfolyam}, szekció: {item.szekció}, név: {item.név}");
				}

				return osztálylista;
			}
		}
	}
}
