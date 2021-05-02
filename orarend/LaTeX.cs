using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace orarend
{
    partial class Program
    {
		public static Dictionary<string, string> tex2ék;
		public static Dictionary<string, string> ék2tex;


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
	}
}
