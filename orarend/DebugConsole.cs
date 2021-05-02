using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace orarend
{
	class DebugConsole
	{
		static public bool debugmode = false;
		static public void WriteLine(Nyom nyom, string str)
		{
			if (debugmode)
			{
				Színes.WriteLine($"{nyom.szöveg} {str}");
			}
		}

		static public void WriteLine(string str)
		{
			if (debugmode)
			{
				Színes.WriteLine(str);
			}
		}

		static public void Write(Nyom nyom, string str)
		{
			if (debugmode)
			{
				Színes.Write($"{nyom.szöveg} {str}");
			}
		}

		static public void Write(string str)
		{
			if (debugmode)
			{
				Színes.Write(str);
			}
		}
	}
}
