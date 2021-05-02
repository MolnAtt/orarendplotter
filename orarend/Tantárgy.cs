using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	partial class Program
	{
		public class Tantárgy
		{

			public static List<Tantárgy> lista = new List<Tantárgy>();
			public string név;


			public Tantárgy(string[] sortömb, Nyom nyom)
			{
				név = sortömb[1];
				lista.Add(this);
				DebugConsole.WriteLine(nyom, $"{név} tantárgy létrehozva");
			}

			public override string ToString() => név;
        }
	}
}
