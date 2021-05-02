using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{
		
		static void Main(string[] args)
		{
			Console.WriteLine("Legyen debug-módban? (\'1\': igen, más: nem)");
			DebugConsole.debugmode = Console.ReadKey().KeyChar == '1';
            Console.WriteLine();
            if (DebugConsole.debugmode)
            {
				Console.WriteLine("Részletezze a sávok szétbontását is? (\'1\': igen, más: nem)");
				Csoport.sávbontás_részletezése = Console.ReadKey().KeyChar == '1';
			}
            Console.WriteLine();
			Console.WriteLine("\nKis türelem...");

			Nyom nyom = new Nyom("Main");
			
			// beolvassuk az órarendet
			// beolvassuk az adatbázisokat, amiket használunk: ékezetek átírása TeX-re, ...
			tex2ék = DB_Beolvas("DB_ékezet.tsv", 1, 0, nyom / "DB_Beolvas:tex2ék");
			ék2tex = DB_Beolvas("DB_ékezet.tsv", 0, 1, nyom / "DB_Beolvas:ék2tex");

			Óra.Init(nyom); // ez a nap2int-et tölti be.
			// Tanárok beolvasása
			Órarend.TanárImport("DB_tanárok.tsv", nyom / "TanárImport");
			
			// Termek beolvasása
			Órarend.TeremImport("DB_termek.tsv", nyom / "TeremImport");

			// Osztályok beolvasása
			Órarend.OsztályImport("DB_osztályok.tsv", nyom / "OsztályImport");

			// Krétás adatbázis beolvasása és a tanárokhoz, termekhez, osztályokhoz csatolása.
			Órarend.AdatbázisImport("DB_kreta.tsv", nyom / "AdatbázisImport");

			Órarend.TanáriÓrarendKészítése(nyom / "TanáriÓrarendKészítése");
			Órarend.TeremÓrarendKészítése(nyom / "TeremÓrarendKészítése");
//			Órarend.Üresteremórarend(nyom / "Üresteremórarend");

			Órarend.DiákÓrarendKészítése(nyom / "DiákÓrarendKészítése");

			Órarend.DiákÓrarendKészítéseHTML(nyom / "DiákÓrarendKészítéseHTML");


			// teszt// Órarend.str2oszt("knyef,9.d", nyomkövetés);

			DebugConsole.WriteLine("===============");
			DebugConsole.WriteLine(" Program vége.");
			DebugConsole.WriteLine("===============");
			DebugConsole.WriteLine(" A következő problémák adódtak: ");
			DebugConsole.WriteLine("===============");

			Probléma.Összes_Kiírása();

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


