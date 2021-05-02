using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{
		public class Terem
		{
			public static List<Terem> lista = new List<Terem>();
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

			public Terem(string[] sortömb, Nyom nyom)
			{
				sorszám = sortömb[0];
				kr = sortömb[1];
				st = sortömb[2];
				emelet = int.Parse(sortömb[3]);
				szárny = sortömb[4];

				lista.Add(this);

				DebugConsole.Write($"[blue]{{({lista.Count})}}"); // nyomkövetés
			}

			public override string ToString() => st;
        }
	}
}
