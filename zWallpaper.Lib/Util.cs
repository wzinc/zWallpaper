using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace zWallpaper.Lib
{
	public static class Util
	{
		#region External
		// this is for updating the background once the wallpaper is set
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetLastError();

		#endregion

		#region Members
		private static readonly ushort SPI_SETDESKWALLPAPER = 0x0014;
		private static readonly ushort SPIF_UPDATEINIFILE = 0001;
		private static readonly ushort SPIF_SENDCHANGE = 0x02;

		private static readonly RegistryKey _desktopRegistryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
		#endregion

		#region SetWallpaper
		/// <summary>
		/// Sets the wallpaper and updates the background.
		/// </summary>
		/// <param name="fileName">path to wallpaper file</param>
		/// <returns>whether the wallpaper was changed</returns>
		public static bool SetWallpaper(string fileName)
		{
			_desktopRegistryKey.SetValue("WallpaperStyle", "2");
			_desktopRegistryKey.SetValue("TileWallpaper", "1");

			return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fileName, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE) > 0;
		}
		#endregion
	}
}