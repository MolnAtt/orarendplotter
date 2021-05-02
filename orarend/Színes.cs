using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	static class Színes
	{
		private static string Intervallum(string s, int a, int b)
		{
			return s.Substring(a + 1, b - a - 3);
		}
		private static void Színtvált(string szín)
		{
			switch (szín)
			{
				case "blue":
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
				case "white":
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case "red":
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case "green":
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case "yellow":
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				default:
					Console.ForegroundColor = ConsoleColor.DarkRed;

					break;
			}
		}
		public static void WriteLine(string s)
		{
			Write(s);
			Console.WriteLine();
		}
		public static void Write(string s)
		{
			int i = s.IndexOf('[');
			int j = s.IndexOf(']');
			int l = s.IndexOf('}');
			if (i == -1 || j == -1 || l == -1)
			{
				Console.Write(s);
			}
			else
			{
				if (i > 0)
				{
					Console.Write(s.Substring(0, i));
				}
				Színtvált(s.Substring(i + 1, j - i - 1
					));
				Console.Write(s.Substring(j + 2, l - j
					- 2
					));
				Színtvált("white");
				if (l < s.Length - 1)
				{
					Színes.Write(s.Substring(l + 1));
				}
			}
		}
	}
}
