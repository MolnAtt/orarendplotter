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
		/// visszaad annyi tabot, amennyit mondunk neki.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
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
	}
}
