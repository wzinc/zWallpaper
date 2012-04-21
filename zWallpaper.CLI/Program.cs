using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zWallpaper.Lib;

namespace zWallpaper.CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Wallpeper changed: {0}", Util.SetWallpaper(@"C:\heliocentric31440p.bmp"));
		}
	}
}