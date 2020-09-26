using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	class Program
	{
		class DebugConsole 
		{
			static public bool debugmode = false;
			static public void WriteLine(string str)
			{
				if (debugmode)
				{
					Console.Error.WriteLine(str);
				}
			}

			static public void Write(string str)
			{
				if (debugmode)
				{
					Console.Error.Write(str);
				}
			}
		}
		/// <summary>
		/// A tanár objektum: Van neve, tex-re átírt neve. Később: Hol lakik, Mennyit lépcsőzhet, mely emeletekre mehet, heti óraszáma, lyukas óráinak száma, mely osztályokat tanít, mely termekben tanít, ...
		/// </summary>
		public static string nyomkövetőjel = "/";
		/// <summary>
		/// visszaad annyi tabot, amennyit mondunk neki.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static List<string> napnév = new List<string> { "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek"};
		public static string Tabolás(int n)
		{
			if (n > 0)
			{
				string result = "\t";
				for (int i = 1; i < n; i++)
				{
					result += "\t";
				}
				return "\n" + result;
			}
			else return "\n";
		}
		public class Tanár
		{
			/// <summary>
			/// A tanári órarendben elfoglalt sorszáma
			/// </summary>
			public string sorszám;
			/// <summary>
			/// Órarendbe való standard név
			/// </summary>
			public string st;
			/// <summary>
			/// Krétában található név
			/// </summary>
			public string kr;
			/// <summary>
			/// Krétában szereplő név
			/// </summary>
			public string mg;
		}
		public class Tantárgy
		{
			public string név;
		}
		public class Osztály
		{
			public int sorszám;
			public string évfolyam; // string, mert NY.F, NY.E, KNY.A	
			public string szekció;
			public string név;
			public int maxtagoltság; // a nagy diákórarendben a lehetséges legtöbb egyszerre tartott órája az osztálynak (pl. ha 11 csoportra bontják valamikor, akkor akár 11!)
		}
		public class Csoport
		{
			public string krétakód;
			public string sávID;
			public string csopID;
			public List<Osztály> osztályok = new List<Osztály>();
		}
		public class Terem
		{
			/// <summary>
			/// krétás név
			/// </summary>
			public string kr;
			
			/// <summary>
			/// standardizált név
			/// </summary>
			public string st;
			
			/// <summary>
			/// teremórarendbeli pozíció
			/// </summary>
			public string sorszám;
			
			/// <summary>
			/// terem emelete
			/// </summary>
			public int emelet;

			/// <summary>
			/// terem szárnya
			/// </summary>
			public string szárny;
		}
		public class Óra
		{
			public Tanár tanár = new Tanár();
			public Tantárgy tantárgy = new Tantárgy();
			public Csoport csoport = new Csoport();
			public int nap;
			public int hanyadik;
			public Terem terem = new Terem();
			public bool UtolsoTerem = false;
		}
		public static Dictionary<string, string> tex2ék;
		public static Dictionary<string, string> ék2tex;

		/// <summary>
		/// Beolvassa az n oszlopos "fajlnev" nevű fájlt és létrehoz az alapján egy szótárt amelynek kulcsait a táblázat "keyCol"-ik oszlop, értékeit pedig a "keyValue"-ik oszlop szolgáltatja.
		/// </summary>
		/// <param name="fajlnev">A fájl, amiben az adatbázis van.</param>
		/// <param name="keyCol">A kulcsokat tartalmazó oszlop sorszáma.</param>
		/// <param name="valueCol">Az értékeket tartalmzó oszlop sorszáma.</param>
		/// <returns>Az a Dictionary, amiben csak ez a két oszlop szerepel beolvasva.</returns>
		public static Dictionary<string, string> DB_Beolvas(string fajlnev, int keyCol, int valueCol, string nyomkövetés)
		{
			// nyomkövetés
			{
				string nyomkövetőID = "DB_Beolvas(" + fajlnev + "," + keyCol.ToString() + "," + valueCol.ToString() + "," + ")";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
			}

			StreamReader beolvaso = new StreamReader(fajlnev);
			Dictionary<string, string> db = new Dictionary<string, string>();
			while (!beolvaso.EndOfStream)
			{
				string[] sor = beolvaso.ReadLine().Split('\t');
				db.Add(sor[keyCol], sor[valueCol]);
			}
			return db;
		}
		/// <summary>
		/// stringfordító: az adott inputot az adott Dictionary alapján lefordítja (pl TeX-es ékezetek oda-vissza)
		/// </summary>
		/// <param name="input">Amit fordítani kell.</param>
		/// <param name="table">A szótár, ami alapján fordítunk.</param>
		/// <returns></returns>
		public static string Translate(string input, Dictionary<string, string> table)
		{
			foreach (string kulcs in table.Keys) { input = input.Replace(kulcs, table[kulcs]); }
			return input;
		}

		public static class Órarend
		{
			// PROPERTIES
			public static List<Óra> adatbázis = new List<Óra>();
			public static List<Tanár> tanárok = new List<Tanár>();
			public static List<Terem> termek = new List<Terem>();
			public static List<Osztály> osztályok = new List<Osztály>();
			public static Dictionary<string, string> customsávkódok;
			/// <summary>
			/// Az osztály neve alapján visszaadja az osztályt magát.
			/// </summary>
			/// <param name="neve">Az osztály neve, pl. "11.f" vagy "nyf"</param>
			/// <returns></returns>
			public static Osztály GetOsztály(string neve) { return osztályok.Where(x => x.név == neve).First(); }
			/// <summary>
			/// Az osztály sorszáma alapján visszaadja az osztályt magát.
			/// </summary>
			/// <param name="neve">Az osztály neve, pl. "11.f" vagy "nyf"</param>
			/// <returns></returns>
			public static Osztály GetOsztály(int sorszám, string nyomkövetés)
			{
				const string nyomkövetőID = "GetOsztály(sorszám)";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
				DebugConsole.WriteLine(nyomkövetés + "Keresem: " + sorszám.ToString());
				Osztály result = null;
				IEnumerable<Osztály> filtered = osztályok.Where(x => x.sorszám == sorszám);
				if (filtered.Count() == 1) result = filtered.First();
				else
				{
					string hibaüzenet = nyomkövetés + "BAJ VAN: ezt nem találtam: " + sorszám.ToString();
					DebugConsole.WriteLine(hibaüzenet);
					Problémák.Add(hibaüzenet);
				}
				return result;
			}
			/// <summary>
			/// Az osztály évfolyama és szekciója alapján visszaadja az osztályt magát.
			/// </summary>
			/// <param name="neve">Az osztály neve, pl. "11.f" vagy "nyf"</param>
			/// <returns></returns>
			public static Osztály GetOsztály(string évf, string szek, string nyomkövetés)
			{
				const string nyomkövetőID = "GetOsztály(évf,szek)";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
				DebugConsole.WriteLine(nyomkövetés + "Keresem: " + évf + szek);
				Osztály result = null;
				IEnumerable<Osztály> filtered = osztályok.Where(x => x.évfolyam == évf && x.szekció == szek);
				if (filtered.Count() == 1) result = filtered.First();
				else
				{
					string hibaüzenet = nyomkövetés + "BAJ VAN: ezt nem találtam: " + évf + szek;
					DebugConsole.WriteLine(hibaüzenet);
					Problémák.Add(hibaüzenet);
				}
				return result;
			}
			// METHODS
			// Órarendi adatbázis

			/// <summary>
			/// Beimportálja a Kódolás munkalapból lementett tsv-fájlt, létrehozza az órákat mint külön objektumokat és az így keletkezett óralista alkotja majd az adatbázist, amire a későbbiekben minden hivatkozik.
			/// </summary>
			/// <param name="fajlnev">A beolvasandó fájl neve</param>
			/// 
			public static void AdatbázisImport(string fajlnev, string nyomkövetés)
			{
				// nyomkövetés
				const string nyomkövetőID = "AdatImp";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
				DebugConsole.WriteLine(nyomkövetés+"("+nyomkövetőID+") indul ...");
				Dictionary<string, string> nap2int = DB_Beolvas("DB_napok.tsv", 0, 1, nyomkövetés);

				StreamReader beolvaso = new StreamReader(fajlnev);
				beolvaso.ReadLine().Split('\t'); // fejlécek nem kellenek.				
				int i = 0;
				while (!beolvaso.EndOfStream)
				{
					string[] sor = beolvaso.ReadLine().Split('\t');
					Óra ora = new Óra
					{
						tanár = tanárok.Where(t => t.kr == sor[0]).First(),
						tantárgy = new Tantárgy { név = sor[1] },
						csoport = new Csoport { sávID = sor[2], csopID = sor[6], osztályok = str2oszt(sor[2], nyomkövetés), krétakód = sor[7] },
						nap = int.Parse(nap2int[sor[3]]),
						hanyadik = int.Parse(sor[4]),
						terem = termek.Where(t => t.kr == sor[5]).First()
					};
					adatbázis.Add(ora);
					DebugConsole.WriteLine(nyomkövetés + $": {i++}. sor beolvasva");
				}
			}
			
			/// <summary>
			/// Minden óráról eldönti, hogy az óra utolsó órája-e az adott teremnek. Ha igen, ezt jelöli.
			/// </summary>
			public static void UtolsoTeremJelölése(){/* under construction*/}
			
			/// <summary>
			/// Betölti a tanárokat tartalmazó adatokat. Ebben szerepel az az információ, hogy mi a tanári órarendben a sorrend, mik a krétás nevekhez tartozó rövidebb nevek és monogramok.
			/// </summary>
			/// <param name="fajlnev">A tanárokról szóló információk adatbázisa.</param>
			public static void TanárImport(string fajlnev, string nyomkövetés)
			{
				const string nyomkövetőID = "TanImp";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
				DebugConsole.WriteLine(nyomkövetés + " TanárImport (" + nyomkövetőID + ") indul");
				StreamReader beolvaso = new StreamReader(fajlnev);
				int i = 0; // nyomkövetés
				while (!beolvaso.EndOfStream)
				{
					string[] sor = beolvaso.ReadLine().Split('\t');
					tanárok.Add(new Tanár
					{
						sorszám = sor[0],
						kr = sor[1],
						st = sor[2],
						mg = sor[3],
					});
					DebugConsole.WriteLine(nyomkövetés + $": {++i}. tanár beolvasva."); // nyomkövetés
				}
				beolvaso.Close();
				DebugConsole.WriteLine(nyomkövetés + "Mostantól létezik a tanáradatbázis.");

				using (StreamWriter f = new StreamWriter("tanarletszam.tex"))
				{
					f.WriteLine($"\\pgfmathsetmacro{{\\tanarletszam}}{{{Órarend.tanárok.Count}}}");
				}
				DebugConsole.WriteLine(nyomkövetés + "tanarletszam.tex-be kiírtam a tanárok létszámát.");
			}

			/// <summary>
			/// Betölti a termeket tartalmazó adatokat. Ebben szerepel az az információ, hogy mi a teremórarendben a sorrend, mik a krétás nevekhez tartozó rövidebb nevek, hanyadik emelet, stb..
			/// </summary>
			/// <param name="fajlnev">A tanárokról szóló információk adatbázisa.</param>
			public static void TeremImport(string fajlnev, string nyomkövetés)
			{
				const string nyomkövetőID = "TerImp";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
				DebugConsole.WriteLine(nyomkövetés+" TeremImport ("+nyomkövetőID+") indul");
				StreamReader beolvaso = new StreamReader(fajlnev);
				int i = 0;
				while (!beolvaso.EndOfStream)
				{
					string[] sor = beolvaso.ReadLine().Split('\t');
					termek.Add(new Terem
					{
						sorszám = sor[0],
						kr = sor[1],
						st = sor[2],
						emelet = int.Parse(sor[3]),
						szárny = sor[4],
					});
					DebugConsole.WriteLine(nyomkövetés+ $": {++i}. terem beolvasva."); // nyomkövetés
				}
				beolvaso.Close();
				DebugConsole.WriteLine(nyomkövetés + "Mostantól létezik a teremadatbázis.");
			}

			/// <summary>
			/// Betölti az osztályokat tartalmazó adatokat. 
			/// </summary>
			/// <param name="fajlnev"></param>
			public static void OsztályImport(string fajlnev, string nyomkövetés)
			{
				// Nyomkövetés
				{
					const string nyomkövetőID = "OszImp";
					nyomkövetés += nyomkövetőID + nyomkövetőjel;
					DebugConsole.WriteLine(nyomkövetés + "Osztályimport (" + nyomkövetőID + ") indul"); // nyomkövetés
				}

				StreamReader beolvaso = new StreamReader(fajlnev);
				int i = 0;
				while (!beolvaso.EndOfStream)
				{
					string[] sor = beolvaso.ReadLine().Split('\t');
					osztályok.Add(new Osztály
					{
						sorszám = int.Parse(sor[0]),
						név = sor[3],
						évfolyam = sor[1].ToUpper(), // Az azonosításhoz mindenképpen nagybetűsítünk! ("NY", "KNY")
						szekció = sor[2].ToUpper()// Az azonosításhoz mindenképpen nagybetűsítünk! ("A", "F", ...)
					});
					DebugConsole.WriteLine(nyomkövetés + $": {++i}. osztály ({sor[2]}) beolvasva."); // nyomkövetés
				}
				beolvaso.Close();
				DebugConsole.WriteLine(nyomkövetés + "Mostantól létezik az osztályadatbázis.");
			}

			/// <summary>
			/// Az osztálykódokból álló listákat az osztályok listáinak elemeiből álló osztálylistává alakítja. 
			/// </summary>
			/// <param name="kód"></param>
			/// <returns></returns>
			public static List<Osztály> str2oszt(string kód, string nyomkövetés) // a kód egy ilyen : "9.abd,10.ef,11.bcd,12.aef"
			{
				// Nyomkövetés
					const string nyomkövetőID = "str2oszt";
					nyomkövetés += nyomkövetőID + nyomkövetőjel;
					DebugConsole.WriteLine(nyomkövetés + "str2oszt (" + nyomkövetőID + ") indul"); // nyomkövetés

				List<Osztály> osztálylista = new List<Osztály>();

				customsávkódok = DB_Beolvas("DB_customsávkódok.tsv", 0, 1, nyomkövetés);

				string[] évfolyamonként = kód.Split(','); // évfolyamonként: [ "9.abd"  ,  "10.ef"  ,  "11.bcd"  ,  "12.aef" ]
				foreach (string évfolyamrész in évfolyamonként) // egy évfolyamrész: "11.bcd"
				{
					string[] évfolyamsplit = évfolyamrész.Split('.');
					if (évfolyamsplit.Length == 1) // ez csak a kny, nye, nyf, oktv, stb. esetében lehet, szóval ez egy custom sávnév kell legyen.
					{
						osztálylista.AddRange(str2oszt(customsávkódok[évfolyamsplit[0]], nyomkövetés)); // csak akkor lehet végtelen kör, ha az adatbázisban kör van!
					}
					else
					{
						string évf = évfolyamsplit[0];
						foreach (char betű in évfolyamsplit[1])
						{
							osztálylista.Add(GetOsztály(évf.ToUpper(), betű.ToString().ToUpper(), nyomkövetés));
						}
					}
				}

				/*nyomkövetés*/
				DebugConsole.WriteLine(nyomkövetés + $"A \"{kód}\" kódból a következő osztályokat fejtettem vissza:");
				foreach (var item in osztálylista)
				{
					DebugConsole.WriteLine(nyomkövetés + $"évfolyam: {item.évfolyam}, szekció: {item.szekció}, név: {item.név}");
				}

				return osztálylista;
			}


			// TOR : tanárórarend
			/// <summary>
			/// Készít egy tanár x (nap + óra) órarendet.
			/// </summary>
			public static void TanáriÓrarendKészítése(string nyomkövetés)
			{
				string nyomkövetőID = "TOR";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				Dictionary<string, string> osztályátíró = DB_Beolvas("DB_osztalyrov.tsv", 0, 1, nyomkövetés);

				using (StreamWriter tanarkiiro = new StreamWriter("TOR-tanarlista.tex"))
				{
					tanarkiiro.WriteLine("\\newcommand{\\tanarlista}{%");
					foreach (Tanár tanár in tanárok)
					{
						bool utolsosor = tanár == tanárok[tanárok.Count - 1];
						string elvalasztojel = utolsosor ? "" : ",";
						string tanarlistasor = tanár.sorszám.ToString() + "/" + Translate(tanár.st, ék2tex) + elvalasztojel;
						tanarkiiro.WriteLine(tanarlistasor);
					}
					tanarkiiro.WriteLine("}");
				}
				using (StreamWriter monogramkiiro = new StreamWriter("TOR-monogramlista.tex"))
				{
					monogramkiiro.WriteLine("\\newcommand{\\monogramlista}{%");
					foreach (Tanár tanár in tanárok)
					{
						bool utolsosor = tanár == tanárok[tanárok.Count - 1];
						string elvalasztojel = utolsosor ? "" : ",";
						string tanarlistasor = tanár.sorszám.ToString() + "/" + Translate(tanár.mg, ék2tex) + elvalasztojel;
						monogramkiiro.WriteLine(tanarlistasor);
					}
					monogramkiiro.WriteLine("}");
				}

				using (StreamWriter abkiiro = new StreamWriter("TORFLESH.tex"))
				{
					foreach (Óra óra in adatbázis)
					{
						string sor = "\\ora{\\mitrakjonle}{" + óra.tanár.sorszám + "}" +
								"{" + Translate(óra.tantárgy.név, ék2tex) + "}" +
								"{\\tabcolsep=0mm\\begin{tabular}{l}" + Translate(óra.csoport.sávID, osztályátíró) + "\\end{tabular}}" +
								"{" + óra.nap.ToString() + "}" +
								"{" + óra.hanyadik.ToString() + "}" +
								"{" + Translate(óra.terem.st, ék2tex) + "}" +
								"{" + óra.csoport.csopID + "}" +
								"{" + (óra.UtolsoTerem ? "draw=black, fill = gray!40, thick" : "") + "}\r\n";
						if (óra.hanyadik < 9) // ez arra való korlát, hogy a Szabados médiája ne akassza ki a rendszert.
						{
							
							DebugConsole.Write(nyomkövetés + sor);
							abkiiro.Write(sor);
						}
						else
						{
							string hibaüzenet = nyomkövetés + " Túl későn van: " + sor;
							DebugConsole.Write(hibaüzenet);
							Problémák.Add(hibaüzenet);
						}
					}
				}
				//Process.Start("pdflatex TanarNezo3.tex"); 
				//Process.Start("pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TanarNezo3.tex");
			}

			// TEOR : teremórarend
			/// <summary>
			/// Készít egy terem x (nap + óra) órarendet.
			/// </summary>
			public static void TeremÓrarendKészítése(string nyomkövetés)
			{
				string nyomkövetőID = "TEOR";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				Dictionary<string, string> osztályátíró = DB_Beolvas("DB_osztalyrov.tsv", 0, 1, nyomkövetés);
				using (StreamWriter tanarkiiro = new StreamWriter("TEOR-teremlista.tex"))
				{
					tanarkiiro.WriteLine("\\newcommand{\\teremlista}{%");
					foreach (Terem terem in termek)
					{
						bool utolsosor = terem == termek[termek.Count - 1];
						string elvalasztojel = utolsosor ? "" : ",";
						string teremlistasor = terem.sorszám.ToString() + "/" + Translate(terem.st, ék2tex) + elvalasztojel;
						tanarkiiro.WriteLine(teremlistasor);
					}
					tanarkiiro.WriteLine("}");
				}
				using (StreamWriter abkiiro = new StreamWriter("TEORFLESH.tex"))
				{
					foreach (Óra óra in adatbázis)
					{
						string sor = "\\ora{\\mitrakjonle}{" + óra.terem.sorszám + "}" +
								"{" + Translate(óra.tantárgy.név, ék2tex) + "}" +
								"{\\tabcolsep=0mm\\begin{tabular}{l}" + Translate(óra.csoport.sávID, osztályátíró) + "\\end{tabular}}" +
								"{" + óra.nap.ToString() + "}" +
								"{" + óra.hanyadik.ToString() + "}" +
								"{" + Translate(óra.tanár.st, ék2tex) + "}" +
								"{" + Translate(óra.csoport.csopID, ék2tex) + "}" +
								"{" + Translate(óra.csoport.krétakód, ék2tex) + "}\r\n"; //ITT
						if (óra.hanyadik < 9) // ez arra való korlát, hogy tömbösített médiák és szakkörök ne akasszák ki a rendszert.
						{							
							DebugConsole.Write(nyomkövetés+sor);
							abkiiro.Write(sor);
						}
						else
						{
							string hibaüzenet = nyomkövetés + " Túl későn van: " + sor;
							DebugConsole.Write(hibaüzenet);
							Problémák.Add(hibaüzenet);
						}
					}
				}

				using (StreamWriter teremszamkiiro = new StreamWriter("teremszam.tex"))
				{
					teremszamkiiro.Write($"\\pgfmathtruncatemacro{{\\teremletszam}}{{{termek.Count}}}");
				}

				using (StreamWriter futtatokiiro = new StreamWriter("TeremNezo31.tex"))
				{
					string parancssor =
						"\\documentclass[a3paper]{article}" +
						"\n\\usepackage[left = 2.75cm, top = 1cm, paper width = 185cm, paper height = 90cm]{geometry}" +
						"\n\\input{TEOR-preambulum3}" +
						"\n\\begin{document}" +
						"\n\\quad" +
						"\n\n" +
						"\n\\vspace{3cm}" +
						"\n\n" +
	  					"\n\\hspace{6cm}\\skeleton" +
						"\n\\orarend{1,...," + termek.Count + "}" +
						"\n\\end{document}";
					futtatokiiro.Write(parancssor);
				}

			}

			/// <summary>
			/// Készít egy óra x nap táblázatot, ahova belistázza az üres termeket.
			/// </summary>
			public static void Üresteremórarend(string nyomkövetés)
			{
				string nyomkövetőID = "ÜTEOR";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				string[,] ütóm = new string[5, 6]; // Üres Terem-Órarend Mátrix
				List<string> nemterem = new List<string> { "Tt.", "dök", "D.", "II.", "tsz", "K.gy", "B.gy", "Szt.2", "Szt.3", "Szt.1", "Tk.", "T.2", "L.1", "L.2", "A.3", "Ol.1", "Ol.2", "Kt."};
				string kiirando = "";
				for (int nap = 0; nap < 5; nap++)
				{
					for (int óra = 0; óra < 6; óra++)
					{
						ütóm[nap,óra] = "";
						foreach (Terem terem in termek)
						{
							if ((adatbázis.Where(x => (x.nap == (nap + 1) && x.hanyadik == (óra + 1) && x.terem == terem)).Count() == 0)
								&& !nemterem.Contains(terem.st))
							{
								ütóm[nap, óra] += terem.st + " ";
							}
						}
						kiirando += "\n\\ora{" + (nap+1) + "}{" + (óra+1) + "}{" + ütóm[nap,óra] + "}";
					}
				}

				DebugConsole.WriteLine(nyomkövetés + kiirando);
				using (StreamWriter utomkiiro = new StreamWriter("UTEORFLESH.tex"))
				{
					utomkiiro.WriteLine(kiirando);
				}

			}


			// DORpreparátor: mindenféle diákórarendhez.
			/// <summary>
			/// A DOR-hoz szét kell szedni az órákat osztályok szerint. Ezt készíti elő a diákórarend-preparátor.
			/// </summary>
			/// <param name="nyomkövetés"></param>
			/// <returns>egy osztályszám x nap x óra 3D-s mátrix</returns>
			public static List<string>[,,] DORPreparátor(string nyomkövetés)
			{
				// nyomkövetés
				string nyomkövetőID = "DORPreparátor";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				List<string>[,,] DORPreparátum = new List<string>[osztályok.Count,5,10];

				// inicializálás
				for (int i = 0; i < osztályok.Count; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							DORPreparátum[i, j, k] = new List<string>();
						}
					}
				}
				DebugConsole.WriteLine(nyomkövetés+"3D mátrix inicializálva");

				foreach (Óra óra in adatbázis)
				{
					if (óra.hanyadik < 9)
					{
						foreach (Osztály osztály in óra.csoport.osztályok)
						{
							string bele =
								" \\oraresz{" + óra.tanár.st + "}" +
										 "{" + óra.tantárgy.név + "}" +
										 "{" + óra.csoport.csopID + "}" +
										 "{" + óra.terem.st + "}";
							DORPreparátum[osztály.sorszám - 1, óra.nap - 1, óra.hanyadik].Add(bele);
							DebugConsole.WriteLine(nyomkövetés + " DOR[" + (osztály.sorszám - 1).ToString() + "," + (óra.nap - 1).ToString() + "," + óra.hanyadik + "] += " + bele);
						}
					}
					else
					{
						string hibaüzenet = nyomkövetés + "Kihagytam a következő órát, mert nem fért be: " + óra.tanár.st + ", "+ óra.tantárgy.név + ", " + óra.csoport.sávID + óra.csoport.csopID + ", " + óra.nap.ToString()+ ", " + óra.hanyadik.ToString();
						DebugConsole.WriteLine(hibaüzenet);
						Problémák.Add(hibaüzenet);
					}
				}

				return DORPreparátum;
			}

			// DORSormérés: csak a nagy diákórarendhez. Ez tölti fel a maxtagoltság tulajdonságot is!
			/// <summary>
			/// Megméri, hogy egy dorpreparátum alapján mekkora sormagasságokkal kell számolni, és egy listában visszaadja.
			/// </summary>
			/// <param name="DorPrep"></param>
			/// <param name="nyomkövetés"></param>
			/// <returns></returns>
			public static void DORSormérés(List<string>[,,] DorPrep, string nyomkövetés)
			{
				string nyomkövetőID = "DORSormérés";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				for (int i = 0; i < osztályok.Count; i++)
				{
					int max = 0;
					for (int j = 0; j < 5; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							if (DorPrep[i,j,k].Count>max)
							{
								max = DorPrep[i, j, k].Count;
							}
						}
					}
					GetOsztály(i+1, nyomkövetés).maxtagoltság = max;
					DebugConsole.WriteLine(nyomkövetés + i.ToString() + ". sor (" + GetOsztály(i+1, nyomkövetés)+ ") megmérve, vastagsága: " + max.ToString());
				}
			}

			// DOR : Diákórarend vagy Osztályórarend
			/// <summary>
			/// Készít egy osztály x (nap + óra) órarendet.
			/// </summary>
			public static void DiákÓrarendKészítése(string nyomkövetés)
			{
				string nyomkövetőID = "DOR";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				/* Itt most előbb létrehozunk egy képzeletbeli órarendet: 
				 * List<string>[,] alakút. 
				 * Ebben az szerepel. hogy majd a végleges órarendben melyik cellába mit kellene írni.
				 * 
				 */
				List<string>[,,] DORPreparátum = DORPreparátor(nyomkövetés);
				// most ki kell szedegetni, hogy melyik milyen magas!
				DORSormérés(DORPreparátum, nyomkövetés); // ez feltölti a maxtagoltság tulajdonságokat az osztályoknál.
														 // A maxtagoltságot elmentjük a tex-es osztálylistába, mert ez igazából egy formázáshoz szükséges tulajdonság.

				using (StreamWriter osztalykiiro = new StreamWriter("DOR-osztalylista.tex"))
				{
					osztalykiiro.WriteLine("\\newcommand{\\osztalylista}{%");
					foreach (Osztály osztály in osztályok)
					{
						bool utolsosor = osztály == osztályok[osztályok.Count - 1];
						string elvalasztojel = utolsosor ? "%" : ",";
						string osztálylistasor = osztály.sorszám.ToString() + "/" + osztály.maxtagoltság.ToString() + "/" + osztály.név + elvalasztojel;
						osztalykiiro.WriteLine(osztálylistasor);
					}
					osztalykiiro.WriteLine("}");
				}

				using (StreamWriter abkiiro = new StreamWriter("DORFLESH.tex")) //ITT TARTOTTAM
				{
					for (int sorszám = 0; sorszám < osztályok.Count; sorszám++)
					{
						for (int napszám = 0; napszám < 5; napszám++)
						{
							for (int óraszám = 0; óraszám < 10; óraszám++)
							{
								string cellavastagsag = (((double)osztályok[sorszám].maxtagoltság) / (double)(DORPreparátum[sorszám, napszám, óraszám].Count)).ToString().Replace(',','.');
								string cellastring = "";
								foreach (string orastring in DORPreparátum[sorszám,napszám,óraszám])
								{
									string elválasztójel = (orastring == DORPreparátum[sorszám, napszám, óraszám].Last()) ? "" : "\r\n\\\\";
									cellastring += orastring + "{" + cellavastagsag + "}%" + elválasztójel;
									DebugConsole.Write(nyomkövetés + cellastring);
								}
								string DORsor = 
									"\\ora{\\mitrakjonle}" +
										 "{" + (sorszám+1).ToString() + "}" +
										 "{" + (napszám+1).ToString() + "}" +
										 "{" + óraszám.ToString() + "}" +
										 "{\\begin{cellatartalom}% \r\n  " + Translate(cellastring, ék2tex) + "%\r\n\\end{cellatartalom}}" +
									"\r\n";
								DebugConsole.WriteLine(nyomkövetés+" fájlba írom: "+ DORsor);
								abkiiro.Write(DORsor);
							}
						}
					}
				}
				//Process.Start("pdflatex TanarNezo3.tex"); 
				//Process.Start("pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TanarNezo3.tex");
			}

			public static void DiákÓrarendKészítéseHTML(string nyomkövetés)
			{
				string nyomkövetőID = "DOR-HTML";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;

				/*A tábla HTML-kódjának elkészítése*/
				DebugConsole.WriteLine(nyomkövetés + "Nekiállok a html-táblának");

				string táblastring ="<table>";
				int t = 1;
				táblastring += Tabolás(t++) + "<thead>";
				táblastring += Tabolás(t) + "<th class = \"sarok\">2019-2020</th>";


				for (int nap = 0; nap < 5; nap++)
				{
					for (int hanyadik = 0; hanyadik < 9; hanyadik++)
					{
						táblastring += Tabolás(t) + "<th class = \"" + napnév[nap] + " ora" + hanyadik + "\" >"
							+ napnév[nap] + " " + hanyadik
							+ "</th>";
					}
				}
				táblastring += Tabolás(--t) + "</thead>";
				táblastring += Tabolás(t) + "<tbody>";

				DebugConsole.WriteLine(nyomkövetés + "table head kész");

				foreach (Osztály osztály in osztályok)
				{
					DebugConsole.WriteLine(nyomkövetés + osztály.név);
					táblastring += Tabolás(t)+ "<tr class = \"" + osztály.név + "\">";
					táblastring += Tabolás(++t) + "<td class=\"osztalycimke\">" + osztály.név +"</td>";
					for (int nap = 1; nap < 6; nap++)
					{
						for (int hanyadik = 0; hanyadik < 9; hanyadik++)
						{
							táblastring += Tabolás(t) + "<td class = \"" + napnév[nap-1] + " ora" + hanyadik + "\" >";
							foreach (Óra óra in adatbázis.Where(ó => (ó.nap == nap && ó.hanyadik == hanyadik && ó.csoport.osztályok.Contains(osztály))))
							{
								táblastring += Tabolás(++t) + "<div class = \"oraitem " + óra.tanár.st + " " + óra.tantárgy.név + " " + óra.csoport.csopID + " " + óra.terem.st + "\">"
								+ Tabolás(++t) + "<span class = \"stanar\">" + óra.tanár.st +"</span>"
								+ Tabolás(t) + "<span class = \"stantargy\">" + óra.tantárgy.név + "</span>"
								+ Tabolás(t) + "<span class = \"scsoport\">" + óra.csoport.csopID + "</span>"
								+ Tabolás(t) + "<span class = \"sterem\">" + óra.terem.st + "</span>"
								+ Tabolás(--t) + "</div>" 
								+ Tabolás(--t);								
							}
							táblastring += "</td>";
						}
					}
					táblastring += Tabolás(--t)+"</tr>";
				}
				táblastring += Tabolás(t) + "</tbody>";
				táblastring += Tabolás(t) + "</table>";
				DebugConsole.WriteLine(nyomkövetés + "Kész a html-tábla");

				File.WriteAllText("orarend.html", File.ReadAllText("pretable.txt") + táblastring + File.ReadAllText("posttable.txt"));
				DebugConsole.WriteLine(nyomkövetés + "Elkészült az orarend.html");

			}
		}

		static List<string> Problémák = new List<string>();

		static void TeX2pdf(string fájlnév, int db)
		{
			string batfájl = "";

			/*
			  pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TanarNezo3.tex
			  pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TanarNezo3Nyomtatott.tex
			  pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TeremNezo.tex
			  pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TeremNezoNyomtatott.tex
			  pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 DiakNezo.tex
			  pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 DiakNezoNyomtatott.tex
			*/

			Dictionary<string, string> TexBatSzótár = new Dictionary<string, string>();
			TexBatSzótár.Add("TanarNezo3.tex", "BuildTanarNezo.bat");
			TexBatSzótár.Add("TanarNezo3Nyomtatott.tex", "BuildTanarNezoNY.bat");
			TexBatSzótár.Add("TeremNezo31.tex", "BuildTeremNezo.bat");
			TexBatSzótár.Add("TeremNezoNyomtatott.tex", "BuildTeremNezoNY.bat");
			TexBatSzótár.Add("DiakNezo.tex", "BuildDiakNezo.bat");
			TexBatSzótár.Add("DiakNezoNyomtatott.tex", "BuildDiakNezoNY.bat");
			
			string hely = Directory.GetCurrentDirectory();
			batfájl = hely + "\\" + TexBatSzótár[fájlnév];
			using (Process process = new Process())
			{
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = false;
				process.StartInfo.WorkingDirectory = hely; // azért kell, mert a futtatandó file ebben a könyvtárban keres majd inputot.
				process.StartInfo.FileName = batfájl;
				for (int i = 0; i < db; i++)
				{
					process.Start();
					process.WaitForExit();
				}
			}
		}
		static void Main(string[] args)
		{
			Console.WriteLine("Legyen debug-módban? (\'1\': igen, más: nem)");
			ConsoleKeyInfo answer = Console.ReadKey();
			DebugConsole.debugmode = answer.KeyChar == '1';
			Console.WriteLine("\nKis türelem...");

			// nyomkövetés -- nyomkövetés
			string nyomkövetés = "";
				string nyomkövetőID = "Main";
				nyomkövetés += nyomkövetőID + nyomkövetőjel;
			
			// beolvassuk az órarendet
			// beolvassuk az adatbázisokat, amiket használunk: ékezetek átírása TeX-re, ...
			tex2ék = DB_Beolvas("DB_ékezet.tsv", 1, 0, nyomkövetés);
			ék2tex = DB_Beolvas("DB_ékezet.tsv", 0, 1, nyomkövetés);

			// Tanárok beolvasása
			Órarend.TanárImport("DB_tanárok.tsv", nyomkövetés);
			
			// Termek beolvasása
			Órarend.TeremImport("DB_termek.tsv", nyomkövetés);

			// Osztályok beolvasása
			Órarend.OsztályImport("DB_osztályok.tsv", nyomkövetés);

			// Krétás adatbázis beolvasása és a tanárokhoz, termekhez, osztályokhoz csatolása.
			Órarend.AdatbázisImport("DB_kreta.tsv", nyomkövetés);

			Órarend.TanáriÓrarendKészítése(nyomkövetés);
			Órarend.TeremÓrarendKészítése(nyomkövetés);
//			Órarend.Üresteremórarend(nyomkövetés);

			Órarend.DiákÓrarendKészítése(nyomkövetés);

			Órarend.DiákÓrarendKészítéseHTML(nyomkövetés);


			// teszt// Órarend.str2oszt("knyef,9.d", nyomkövetés);

			DebugConsole.WriteLine("===============");
			DebugConsole.WriteLine(" Program vége.");
			DebugConsole.WriteLine("===============");
			DebugConsole.WriteLine(" A következő problémák adódtak: ");
			DebugConsole.WriteLine("===============");
			foreach (var item in Problémák)
			{
				DebugConsole.WriteLine(item);
			}

			Console.WriteLine("\nVége, nyomj egy gombot!");
			Console.ReadKey();
			// Tesztek:
			//	string x = "Árvíztűrő tükörfúrógép";
			//	Konzol.WriteLine(Translate(x, ék2tex));

			Console.WriteLine("\nHányszor compile-oljam a TANÁROK digitális órarendjét?");
			int TanárDCompNum = int.Parse(Console.ReadKey().KeyChar.ToString());
			Console.WriteLine("\nHányszor compile-oljam a TANÁROK nyomtatható órarendjét?");
			int TanárPCompNum = int.Parse(Console.ReadKey().KeyChar.ToString());
			Console.WriteLine("\nHányszor compile-oljam a TERMEK digitális órarendjét?");
			int TeremDCompNum = int.Parse(Console.ReadKey().KeyChar.ToString());
			Console.WriteLine("\nHányszor compile-oljam a TERMEK nyomtatható órarendjét?");
			int TeremPCompNum = int.Parse(Console.ReadKey().KeyChar.ToString());
			Console.WriteLine("\nHányszor compile-oljam a DIÁKOK digitális órarendjét?");
			int DiákDCompNum = int.Parse(Console.ReadKey().KeyChar.ToString());
			Console.WriteLine("\nHányszor compile-oljam a DIÁKOK nyomtatható órarendjét?");
			int DiákPCompNum = int.Parse(Console.ReadKey().KeyChar.ToString());

			Console.WriteLine("\nMehet? ('1': igen, más: nem )");
			if (Console.ReadKey().KeyChar=='1')
			{
				Console.WriteLine();
				TeX2pdf("TanarNezo3.tex", TanárDCompNum);
				TeX2pdf("TanarNezo3Nyomtatott.tex", TanárPCompNum);
				TeX2pdf("TeremNezo31.tex", TeremDCompNum);
				TeX2pdf("TeremNezoNyomtatott.tex", TeremPCompNum);
				TeX2pdf("DiakNezo.tex", DiákDCompNum);
				TeX2pdf("DiakNezoNyomtatott.tex", DiákPCompNum);
			}
		}
	}
}


