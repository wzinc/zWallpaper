using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using zWallpaper.Properties;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace zWallpaper
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// check to see if zWallapper is already running
			if ((from p in Process.GetProcessesByName("zWallpaper") select p).Count() > 1)
			{
				MessageBox.Show("Another instance of zWallapper is already running.");
				return;
			}

			Application.Run(new zWallpaperContext());
		}
	}
}