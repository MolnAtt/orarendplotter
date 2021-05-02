using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{
		/// <summary>
		/// A tanár objektum: Van neve, tex-re átírt neve. Később: Hol lakik, Mennyit lépcsőzhet, mely emeletekre mehet, heti óraszáma, lyukas óráinak száma, mely osztályokat tanít, mely termekben tanít, ...
		/// </summary>
		public class Tanár
		{
			public static List<Tanár> lista = new List<Tanár>();
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

			public Tanár(string[] sortömb, Nyom nyom)
			{
				sorszám = sortömb[0];
				kr = sortömb[1];
				st = sortömb[2];
				mg = sortömb[3];
				lista.Add(this);
				DebugConsole.Write($"[blue]{{({lista.Count})}}"); // nyomkövetés
			}

			public override string ToString() => $"({sorszám}) {st} ({mg})";
        }
	}
}
