using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace orarend
{
	partial class Program
	{

		public static class Órarend
		{
			public static List<string> napnév = new List<string> { "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek" };


			/// <summary>
			/// Beimportálja a Kódolás munkalapból lementett tsv-fájlt, létrehozza az órákat mint külön objektumokat és az így keletkezett óralista alkotja majd az adatbázist, amire a későbbiekben minden hivatkozik.
			/// </summary>
			/// <param name="fajlnev">A beolvasandó fájl neve</param>
			/// 
			public static void AdatbázisImport(string fajlnev, Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, "indul");

				foreach (Óra óra in File.ReadAllLines(fajlnev)
											.Skip(1)
											.Select(s => new Óra(s.Split('\t'), nyom / "Óra")))
				{
					DebugConsole.Write(óra.ToString());
				}
				Console.WriteLine();

				DebugConsole.WriteLine(nyom, $" Mostantól létezik a teremadatbázis: [blue]{{{Terem.lista.Count}}} db terem");
			}

			/// <summary>
			/// Minden óráról eldönti, hogy az óra utolsó órája-e az adott teremnek. Ha igen, ezt jelöli.
			/// </summary>
			public static void UtolsoTeremJelölése() {/* under construction*/}

			/// <summary>
			/// Betölti a tanárokat tartalmazó adatokat. Ebben szerepel az az információ, hogy mi a tanári órarendben a sorrend, mik a krétás nevekhez tartozó rövidebb nevek és monogramok.
			/// </summary>
			/// <param name="fajlnev">A tanárokról szóló információk adatbázisa.</param>
			public static void TanárImport(string fajlnev, Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, $"A következő tanárokat olvasom be: ");


                foreach (Tanár tanár in File.ReadAllLines(fajlnev, Encoding.UTF8)
											.Select(sor => new Tanár(sor.Split('\t'), nyom)))
                {
					DebugConsole.Write($"{tanár.mg},");
                }
                Console.WriteLine();

				DebugConsole.WriteLine(nyom,$"Mostantól létezik a tanáradatbázis: [blue]{{{Tanár.lista.Count}}} fő");

				using (StreamWriter f = new StreamWriter("tanarletszam.tex"))
				{
					f.WriteLine($"\\pgfmathsetmacro{{\\tanarletszam}}{{{Tanár.lista.Count}}}");
				}
				DebugConsole.WriteLine(nyom, $"tanarletszam.tex-be kiírtam a tanárok létszámát: [blue]{{{Tanár.lista.Count}}}.");
			}

			/// <summary>
			/// Betölti a termeket tartalmazó adatokat. Ebben szerepel az az információ, hogy mi a teremórarendben a sorrend, mik a krétás nevekhez tartozó rövidebb nevek, hanyadik emelet, stb..
			/// </summary>
			/// <param name="fajlnev">A tanárokról szóló információk adatbázisa.</param>
			public static void TeremImport(string fajlnev, Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, $"indul");


				foreach (Terem terem in File.ReadAllLines(fajlnev, Encoding.UTF8)
					.Select(sor => new Terem(sor.Split('\t'), nyom / "Terem")))
				{
					DebugConsole.Write($"{terem.st},");
				}
				Console.WriteLine();
				
				DebugConsole.WriteLine(nyom, $" Mostantól létezik a teremadatbázis: [blue]{{{Terem.lista.Count}}} db terem");
			}

			/// <summary>
			/// Betölti az osztályokat tartalmazó adatokat. 
			/// </summary>
			/// <param name="fajlnev"></param>
			public static void OsztályImport(string fajlnev, Nyom nyom)
			{
				// Nyomkövetés
				{
					DebugConsole.WriteLine(nyom, $"indul"); // nyomkövetés
				}

				foreach (Osztály osztály in File.ReadAllLines(fajlnev, Encoding.Default)
					.Select(s => new Osztály(s.Split('\t'), nyom)))
				{
					DebugConsole.Write($"{osztály.név},");
				}
				Console.WriteLine();

				DebugConsole.WriteLine(nyom, $" Mostantól létezik az osztályadatbázis: [blue]{{{Osztály.lista.Count}}} db osztály");
			}

			


			// TOR : tanárórarend
			/// <summary>
			/// Készít egy tanár x (nap + óra) órarendet.
			/// </summary>
			public static void TanáriÓrarendKészítése(Nyom nyom)
			{
				
				Dictionary<string, string> osztályátíró = DB_Beolvas("DB_osztalyrov.tsv", 0, 1, nyom/"DB_Beolvas:osztályátíró");

				using (StreamWriter tanarkiiro = new StreamWriter("TOR-tanarlista.tex"))
				{
					tanarkiiro.WriteLine("\\newcommand{\\tanarlista}{%");
					foreach (Tanár tanár in Tanár.lista)
					{
						bool utolsosor = tanár == Tanár.lista.Last();
						string elvalasztojel = utolsosor ? "" : ",";
						string tanarlistasor = $"{tanár.sorszám}/{Translate(tanár.st, ék2tex)}{elvalasztojel}";
						tanarkiiro.WriteLine(tanarlistasor);
					}
					tanarkiiro.WriteLine("}");
				}

				using (StreamWriter monogramkiiro = new StreamWriter("TOR-monogramlista.tex"))
				{
					monogramkiiro.WriteLine("\\newcommand{\\monogramlista}{%");
					foreach (Tanár tanár in Tanár.lista)
					{
						bool utolsosor = tanár == Tanár.lista.Last();
						string elvalasztojel = utolsosor ? "" : ",";
						string tanarlistasor = $"{tanár.sorszám}/{Translate(tanár.mg, ék2tex)}{elvalasztojel}";
						monogramkiiro.WriteLine(tanarlistasor);
					}
					monogramkiiro.WriteLine("}");
				}

				using (StreamWriter abkiiro = new StreamWriter("TORFLESH.tex"))
				{
					foreach (Óra óra in Óra.lista)
					{
						string sor = $"\\ora{{\\mitrakjonle}}{{{óra.tanár.sorszám}}}" +
								$"{{{Translate(óra.tantárgy.név, ék2tex)}}}" +
								$"{{\\tabcolsep=0mm\\begin{{tabular}}{{l}}{Translate(óra.csoport.sávID, osztályátíró)}\\end{{tabular}}}}" +
								$"{{{óra.nap.ToString()}}}" +
								$"{{{óra.hanyadik.ToString()}}}" +
								$"{{{Translate(óra.terem.st, ék2tex)}}}" +
								$"{{{óra.csoport.csopID}}}" +
								$"{{{(óra.UtolsoTerem ? "draw=black, fill = gray!40, thick" : "")}}}\r\n";
						if (óra.hanyadik < 9) // ez arra való korlát, hogy a Szabados médiája ne akassza ki a rendszert.
						{
							DebugConsole.Write(nyom, sor);
							abkiiro.Write(sor);
						}
						else
							new Probléma(nyom, $" Túl későn van:  {sor}");
					}
				}
				//Process.Start("pdflatex TanarNezo3.tex"); 
				//Process.Start("pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TanarNezo3.tex");
			}

			// TEOR : teremórarend
			/// <summary>
			/// Készít egy terem x (nap + óra) órarendet.
			/// </summary>
			public static void TeremÓrarendKészítése(Nyom nyom)
			{

				DebugConsole.WriteLine(nyom, "indul");

				Dictionary<string, string> osztályátíró = DB_Beolvas("DB_osztalyrov.tsv", 0, 1, nyom/"DB_Beolvas:osztályátíró");
				using (StreamWriter tanarkiiro = new StreamWriter("TEOR-teremlista.tex"))
				{
					tanarkiiro.WriteLine(@"\newcommand{\teremlista}{%");
					foreach (Terem terem in Terem.lista)
					{
						bool utolsosor = terem == Terem.lista.Last();
						string elvalasztojel = utolsosor ? "" : ",";
						string teremlistasor = $"{terem.sorszám}/{Translate(terem.st, ék2tex)}{elvalasztojel}";
						tanarkiiro.WriteLine(teremlistasor);
					}
					tanarkiiro.WriteLine("}");
				}
				using (StreamWriter abkiiro = new StreamWriter("TEORFLESH.tex"))
				{
					foreach (Óra óra in Óra.lista)
					{
						string sor = $"\\ora{{\\mitrakjonle}}{{{óra.terem.sorszám}}}" +
								$"{{{Translate(óra.tantárgy.név, ék2tex)}}}" +
								$"{{\\tabcolsep=0mm\\begin{{tabular}}{{l}}{Translate(óra.csoport.sávID, osztályátíró)}\\end{{tabular}}}}" +
								$"{{{óra.nap}}}" +
								$"{{{óra.hanyadik}}}" +
								$"{{{Translate(óra.tanár.st, ék2tex)}}}" +
								$"{{{Translate(óra.csoport.csopID, ék2tex)}}}" +
								$"{{{Translate(óra.csoport.krétakód, ék2tex)}}}\r\n";
						if (óra.hanyadik < 9) // ez arra való korlát, hogy tömbösített médiák és szakkörök ne akasszák ki a rendszert.
						{
							DebugConsole.Write(nyom, sor);
							abkiiro.Write(sor);
						}
						else
							new Probléma(nyom, $" Túl későn van: {sor}");
					}
				}

				using (StreamWriter teremszamkiiro = new StreamWriter("teremszam.tex"))
				{
					teremszamkiiro.Write($"\\pgfmathtruncatemacro{{\\teremletszam}}{{{Terem.lista.Count}}}");
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
						$"\n\\orarend{{1,...,{Terem.lista.Count}}}" +
						"\n\\end{document}";
					futtatokiiro.Write(parancssor);
				}

			}

			/// <summary>
			/// Készít egy óra x nap táblázatot, ahova belistázza az üres termeket.
			/// </summary>
			public static void Üresteremórarend(Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, "indul");

				string[,] ütóm = new string[5, 6]; // Üres Terem-Órarend Mátrix
				List<string> nemterem = new List<string> { "Tt.", "dök", "D.", "II.", "tsz", "K.gy", "B.gy", "Szt.2", "Szt.3", "Szt.1", "Tk.", "T.2", "L.1", "L.2", "A.3", "Ol.1", "Ol.2", "Kt." };
				string kiirando = "";
				for (int nap = 0; nap < 5; nap++)
				{
					for (int óra = 0; óra < 6; óra++)
					{
						ütóm[nap, óra] = "";
						foreach (Terem terem in Terem.lista)
						{
							if ((0 == Óra.lista.Count(x => (x.nap == (nap + 1) && x.hanyadik == (óra + 1) && x.terem == terem)))
								&& !nemterem.Contains(terem.st))
							{
								ütóm[nap, óra] += terem.st + " ";
							}
						}
						kiirando += $"\n\\ora{{{(nap + 1)}}}{{{(óra + 1)}}}{{{ütóm[nap, óra]}}}";
					}
				}

				DebugConsole.WriteLine(nyom, kiirando);
				File.WriteAllText("UTEORFLESH.tex", kiirando);
				/*
				using (StreamWriter utomkiiro = new StreamWriter("UTEORFLESH.tex"))
				{
					utomkiiro.WriteLine(kiirando);
				}
				*/

			}


			// DORpreparátor: mindenféle diákórarendhez.
			/// <summary>
			/// A DOR-hoz szét kell szedni az órákat osztályok szerint. Ezt készíti elő a diákórarend-preparátor.
			/// </summary>
			/// <param name="nyomkövetés"></param>
			/// <returns>egy osztályszám x nap x óra 3D-s mátrix</returns>
			public static List<string>[,,] DORPreparátor(Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, "indul");

				List<string>[,,] DORPreparátum = new List<string>[Osztály.lista.Count, 5, 10];

				// inicializálás
				for (int i = 0; i < Osztály.lista.Count; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							DORPreparátum[i, j, k] = new List<string>();
						}
					}
				}
				DebugConsole.WriteLine(nyom, "3D mátrix inicializálva");

				foreach (Óra óra in Óra.lista)
				{
					if (óra.hanyadik < 9)
					{
						foreach (Osztály osztály in óra.csoport.osztályok)
						{
							string bele =
								$" \\oraresz{{{óra.tanár.st}}}{{{óra.tantárgy.név}}}{{{óra.csoport.csopID}}}{{{óra.terem.st}}}";
							DORPreparátum[osztály.sorszám - 1, óra.nap - 1, óra.hanyadik].Add(bele);
							DebugConsole.WriteLine(nyom, $" DOR[{osztály.sorszám - 1},{óra.nap - 1},{óra.hanyadik}] += {bele}");
						}
					}
					else
						new Probléma(nyom, $" Kihagytam a következő órát, mert nem fért be: {óra.tanár.st}, {óra.tantárgy.név}, {óra.csoport.sávID}{óra.csoport.csopID}, {óra.nap}, {óra.hanyadik}");
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
			public static void DORSormérés(List<string>[,,] DorPrep, Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, "indul");

				for (int i = 0; i < Osztály.lista.Count; i++)
				{
					int max = 0;
					for (int j = 0; j < 5; j++)
					{
						for (int k = 0; k < 10; k++)
						{
							if (DorPrep[i, j, k].Count > max)
							{
								max = DorPrep[i, j, k].Count;
							}
						}
					}

					Osztály osztály;
					if (Osztály.TryGet(i + 1, nyom, out osztály))
						osztály.maxtagoltság = max;
					else
						new Probléma(nyom, $" BAJ VAN: ezt nem találtam: {i+1}");

					DebugConsole.WriteLine(nyom, $" {i}. sor ({osztály}) megmérve, vastagsága: {max}");
				}
			}

			// DOR : Diákórarend vagy Osztályórarend
			/// <summary>
			/// Készít egy osztály x (nap + óra) órarendet.
			/// </summary>
			public static void DiákÓrarendKészítése(Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, "indul");


				/* Itt most előbb létrehozunk egy képzeletbeli órarendet: 
				 * List<string>[,] alakút. 
				 * Ebben az szerepel. hogy majd a végleges órarendben melyik cellába mit kellene írni.
				 * 
				 */

				List<string>[,,] DORPreparátum = DORPreparátor(nyom);
				// most ki kell szedegetni, hogy melyik milyen magas!
				DORSormérés(DORPreparátum, nyom); // ez feltölti a maxtagoltság tulajdonságokat az osztályoknál.
														 // A maxtagoltságot elmentjük a tex-es osztálylistába, mert ez igazából egy formázáshoz szükséges tulajdonság.

				using (StreamWriter osztalykiiro = new StreamWriter("DOR-osztalylista.tex"))
				{
					osztalykiiro.WriteLine(@"\newcommand{\osztalylista}{%");
					foreach (Osztály osztály in Osztály.lista)
					{
						bool utolsosor = osztály == Osztály.lista.Last();
						string elvalasztojel = utolsosor ? "%" : ",";
						string osztálylistasor = $"{osztály.sorszám}/{osztály.maxtagoltság}/{osztály.név}{elvalasztojel}";
						osztalykiiro.WriteLine(osztálylistasor);
					}
					osztalykiiro.WriteLine("}");
				}

				using (StreamWriter abkiiro = new StreamWriter("DORFLESH.tex")) //ITT TARTOTTAM
				{
					for (int sorszám = 0; sorszám < Osztály.lista.Count; sorszám++)
					{
						for (int napszám = 0; napszám < 5; napszám++)
						{
							for (int óraszám = 0; óraszám < 10; óraszám++)
							{
								string cellavastagsag = (((double)Osztály.lista[sorszám].maxtagoltság) / (double)(DORPreparátum[sorszám, napszám, óraszám].Count)).ToString().Replace(',', '.');
								string cellastring = "";
								foreach (string orastring in DORPreparátum[sorszám, napszám, óraszám])
								{
									string elválasztójel = (orastring == DORPreparátum[sorszám, napszám, óraszám].Last()) ? "" : "\r\n\\\\";
									cellastring += $"{orastring}{{{cellavastagsag}}}%{elválasztójel}";
									DebugConsole.Write(nyom, cellastring);
								}
								string DORsor =
									$"\\ora{{\\mitrakjonle}}{{{sorszám + 1}}}{{{napszám + 1}}}{{{óraszám}}}\n  {{\\begin{{cellatartalom}}%\n    {Translate(cellastring, ék2tex)}%\n  \\end{{cellatartalom}}}}\n";
								//DebugConsole.WriteLine(nyom, " fájlba írom: " + DORsor);
								abkiiro.Write(DORsor);
							}
						}
					}
				}
				//Process.Start("pdflatex TanarNezo3.tex"); 
				//Process.Start("pdflatex --enable-write18 --extra-mem-bot=10000000 --synctex=1 TanarNezo3.tex");
			}

			public static void DiákÓrarendKészítéseHTML(Nyom nyom)
			{
				DebugConsole.WriteLine(nyom, "indul");


				/*A tábla HTML-kódjának elkészítése*/
				DebugConsole.WriteLine(nyom, $" Nekiállok a html-táblának");

				string táblastring = "<table>";
				int t = 1;
				táblastring += $"{Tabolás(t++)}<thead>";
				táblastring += $"{Tabolás(t)}<th class = \"sarok\">2019-2020</th>";


				for (int nap = 0; nap < 5; nap++)
				{
					for (int hanyadik = 0; hanyadik < 9; hanyadik++)
					{
						táblastring += $"{Tabolás(t)}<th class = \"{napnév[nap]} ora{hanyadik}\" >{napnév[nap]} {hanyadik}</th>";
					}
				}
				táblastring += $"{Tabolás(--t)}</thead>";
				táblastring += $"{Tabolás(t)}<tbody>";

				DebugConsole.WriteLine(nyom, "table head kész");

				foreach (Osztály osztály in Osztály.lista)
				{
					DebugConsole.WriteLine(nyom, osztály.név);
					táblastring += $"{Tabolás(t)}<tr class = \"{osztály.név}\">";
					táblastring += $"{Tabolás(++t)}<td class=\"osztalycimke\">{osztály.név}</td>";
					for (int nap = 1; nap < 6; nap++)
					{
						for (int hanyadik = 0; hanyadik < 9; hanyadik++)
						{
							táblastring += $"{Tabolás(t)}<td class = \"{napnév[nap - 1]} ora{hanyadik}\" >";
							foreach (Óra óra in Óra.lista.Where(ó => (ó.nap == nap && ó.hanyadik == hanyadik && ó.csoport.osztályok.Contains(osztály))))
							{
								táblastring += $"{Tabolás(++t)} <div class = \"oraitem {óra.tanár.st} {óra.tantárgy.név} {óra.csoport.csopID} {óra.terem.st}\">"
								+ $"{Tabolás(++t)}<span class = \"stanar\">{óra.tanár.st}</span>"
								+ $"{Tabolás(t)}<span class = \"stantargy\">{óra.tantárgy.név}</span>"
								+ $"{Tabolás(t)}<span class = \"scsoport\">{óra.csoport.csopID}</span>"
								+ $"{Tabolás(t)}<span class = \"sterem\">{óra.terem.st}</span>"
								+ $"{Tabolás(--t)}</div>"
								+ Tabolás(--t);
							}
							táblastring += "</td>";
						}
					}
					táblastring += Tabolás(--t) + "</tr>";
				}
				táblastring += Tabolás(t) + "</tbody>";
				táblastring += Tabolás(t) + "</table>";
				DebugConsole.WriteLine(nyom, "Kész a html-tábla");

				File.WriteAllText("orarend.html", File.ReadAllText("pretable.txt") + táblastring + File.ReadAllText("posttable.txt"));
				DebugConsole.WriteLine(nyom, "Elkészült az orarend.html");

			}
		}
	}
}
