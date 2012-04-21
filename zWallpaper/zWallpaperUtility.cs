using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace zWallpaper
{
	/// <summary>
	/// This class prety much handles all the work for zWallpaper.
	/// </summary>
	public static class zWallpaperUtility
	{
		#region External
		// this is for updating the background once the wallpaper is set
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		private const int SPI_SETDESKTOPWALLPAPER = 20;
		private const int SPIF_UPDATEINIFILTER = 0x01;
		private const int SPIF_SENDWININICHANGE = 0x02;
		#endregion

		#region Members / Static Constructor
		private static RegistryKey R_DESKTOP;

		static zWallpaperUtility()
		{
			R_DESKTOP = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
		}
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets the current wallpaper path.
		/// </summary>
		public static string CurrentImagePath
		{
			get { return R_DESKTOP.GetValue("Wallpaper").ToString(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the app will be tun at startup / login.
		/// </summary>
		public static bool RunAtStartup
		{
			get { return File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "zWallpaper.lnk")); }
			set
			{
				string startUpItemPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "zWallpaper.lnk");

				if (value)
				{
					// already created
					if (RunAtStartup)
						return;

					// create shortcut using WSH
					IWshRuntimeLibrary.WshShellClass wshShell = new IWshRuntimeLibrary.WshShellClass();
					IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(startUpItemPath);
					shortcut.TargetPath = Application.ExecutablePath;
					shortcut.Description = "Launch zWallpaper Background Manager";
					shortcut.IconLocation = Application.ExecutablePath;
					shortcut.Save();
				}
				else if (RunAtStartup)
					File.Delete(startUpItemPath);
			}
		}

		/// <summary>
		/// Gets the size of a rectangle that would encompass all screens.
		/// </summary>
		public static Size ScreenSize
		{
			get
			{
				int lowestX = Screen.AllScreens.Min(w => w.Bounds.X),
				lowestY = Screen.AllScreens.Min(w => w.Bounds.Y);
				int highestX = Screen.AllScreens.Max(w => w.Bounds.X + w.Bounds.Width),
					highestY = Screen.AllScreens.Max(w => w.Bounds.Y + w.Bounds.Height);

				return new Size(highestX + Math.Abs(lowestX), highestY + Math.Abs(lowestY));
			}
		}
		#endregion

		#region SetWallpaper
		/// <summary>
		/// Sets the wallpaper and updates the background.
		/// </summary>
		/// <param name="compositeFileName">path to wallpaper file</param>
		/// <param name="error">exception</param>
		/// <returns>a value indicating whether or not the wallpaper was set successfully</returns>
		public static bool SetWallpaper(string compositeFileName, out Exception error)
		{
			try
			{
				error = null;
				R_DESKTOP.SetValue("WallpaperStyle", "2");
				R_DESKTOP.SetValue("TileWallpaper", "1");

				SystemParametersInfo(SPI_SETDESKTOPWALLPAPER, 0, compositeFileName, SPIF_UPDATEINIFILTER | SPIF_SENDWININICHANGE);
			}
			catch (Exception ex) { error = ex; }

			return error == null;
		}

		/// <summary>
		/// Saves an image to the default location and returns the file name.
		/// </summary>
		/// <param name="wallpaper">image to save</param>
		/// <param name="error">exception</param>
		/// <returns>image file name</returns>
		public static string SaveCompositeImage(Image wallpaper, out Exception error)
		{
			try
			{
				error = null;

				// save wallpaper to our default locaion
				string fullName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "_compositeWallpaper.bmp");
				wallpaper.Save(fullName, ImageFormat.Bmp);

				// hide image file
				File.SetAttributes(fullName, File.GetAttributes(fullName) | FileAttributes.Hidden);

				return fullName;
			}
			catch (Exception ex) { error = ex; }

			return null;
		}

		/// <summary>
		/// Builds, saves, and sets the wallpaper based on the current ScreenSettingsCollection.
		/// </summary>
		/// <param name="error">exception</param>
		public static void UpdateWallpaper(out Exception error)
		{
			// get image
			string wallpaperPath = null;
			using (Image wallpaper = GenerateCompositeBackground(out error))
			{
				// save
				if (error == null)
					wallpaperPath = SaveCompositeImage(wallpaper, out error);

				// set
				if (wallpaperPath != null && error == null)
					SetWallpaper(wallpaperPath, out error);
			}

			// free-up memory for those wanting to run zWallpaper in the task tray.
			GC.Collect();
		}
		#endregion

		#region Image Composition
		/// <summary>
		/// Gets a Screen instance from the provided screen ScreenSettings.
		/// </summary>
		/// <param name="screenSettings">screen settings</param>
		/// <returns>Screen instance</returns>
		public static Screen GetScreen(ScreenSettings screenSettings)
		{
			var screens = from screen in Screen.AllScreens where screen.DeviceName == screenSettings.ScreenID select screen;

			return screens.Count() == 1 ? screens.First() : null;
		}

		/// <summary>
		/// Draw image for a single screen using the provided ScreenSettings.
		/// </summary>
		/// <param name="screenSettings">screen settings</param>
		/// <returns>background image</returns>
		public static Image DesktopImage(ScreenSettings screenSettings)
		{
			Screen screen = GetScreen(screenSettings);
			Image image = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
			Graphics graphics = Graphics.FromImage(image);

			if (!string.IsNullOrEmpty(screenSettings.WallpaperPath))
			{
				// give a better error for missing files
				if (!File.Exists(screenSettings.WallpaperPath))
					throw new FileNotFoundException(string.Format("The file '{0}' could not be found.", screenSettings.WallpaperPath),
						screenSettings.WallpaperPath);

				Image wallpaper = Image.FromFile(screenSettings.WallpaperPath);

				float ratio;
				Rectangle newImageBounds = Rectangle.Empty;

				// test to see if the image is the same size as the screen
				//	this should happen often enough that I thought I'd make it more efficient if there was an exact match
				//	all that math isn't necessary if the screen and image are the same size
				bool imageIsSameAsScreenSize = wallpaper.Width != screen.Bounds.Width || wallpaper.Height != screen.Bounds.Height;

				if (!imageIsSameAsScreenSize)
				{
					// get image resize ratio
					ratio = Math.Min((float)screen.Bounds.Height / (float)wallpaper.Height, (float)screen.Bounds.Width / (float)wallpaper.Width);

					// get the image bounds based on the new ratio
					newImageBounds = new Rectangle(screen.Bounds.Width / 2 - (int)((float)(wallpaper.Width / 2) * ratio), screen.Bounds.Height / 2 - (int)((float)
						(wallpaper.Height / 2) * ratio), (int)((float)wallpaper.Width * ratio), (int)((float)wallpaper.Height * ratio));
				}
				
				switch (screenSettings.Style)
				{
					case WallPaperStyle.FitToScreen:
						
						if (!imageIsSameAsScreenSize)
						{
							// fill background color
							graphics.FillRectangle(new SolidBrush(screenSettings.BackgroundColor), 0, 0, screen.Bounds.Width, screen.Bounds.Height);
							
							graphics.DrawImage(wallpaper, newImageBounds);
						}
						else
							graphics.DrawImage(wallpaper, new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height));
						break;
					case WallPaperStyle.FillScreen:
						if (!imageIsSameAsScreenSize)
						{
							// fill background color
							graphics.FillRectangle(new SolidBrush(screenSettings.BackgroundColor), 0, 0, screen.Bounds.Width, screen.Bounds.Height);

							graphics.DrawImage(wallpaper, newImageBounds);
						}
						else
							graphics.DrawImage(wallpaper, new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height));
						break;
					case WallPaperStyle.StretchToFillScreen:
						graphics.DrawImage(wallpaper, new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height));
						break;
					case WallPaperStyle.Center:
						// only fill if image is smaller than background
						if (screen.Bounds.Width > wallpaper.Width && screen.Bounds.Height > wallpaper.Height)
							graphics.FillRegion(new SolidBrush(screenSettings.BackgroundColor), new Region(screen.Bounds));

						graphics.DrawImage(wallpaper, screen.Bounds.Width / 2 - wallpaper.Width / 2, screen.Bounds.Height / 2 - wallpaper.Height / 2,
							wallpaper.Width, wallpaper.Height);
						break;
					case WallPaperStyle.Tile:
						// create new texture brush
						TextureBrush texture = new TextureBrush(wallpaper, WrapMode.Tile);

						// if image is not same as screen size, translate texture to start from the center
						if (!imageIsSameAsScreenSize)
							texture.TranslateTransform(screen.Bounds.Width / 2 - wallpaper.Width / 2, screen.Bounds.Height / 2 - wallpaper.Height / 2);

						graphics.FillRegion(texture, new Region(new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height)));
						break;
				}
			}
			else
				// no image, just color fill
				graphics.FillRegion(new SolidBrush(screenSettings.BackgroundColor), new Region(screen.Bounds));

			return image;
		}

		/// <summary>
		/// Creates a full, multi-screen desktop background.
		/// </summary>
		/// <param name="error">exception</param>
		/// <returns>full background image</returns>
		public static Image GenerateCompositeBackground(out Exception error)
		{
			error = null;
			Size screenSize = ScreenSize;

			// create a new image of the size of a rectangle that would encompass all screens
			Image wallpaper = new Bitmap(screenSize.Width, screenSize.Height);
			Graphics graphics = Graphics.FromImage(wallpaper);

			try
			{
				Point start = new Point();
				// find starting point by adding the dimentions of all screens above and to the left of the main screen
				foreach (KeyValuePair<string, ScreenSettings> screenSettings in ScreenSettingsCollection.Current)
				{
					Screen screen = zWallpaperUtility.GetScreen(screenSettings.Value);

					// skip screen if removed
					if (screen == null)
						continue;

					if (screen.Bounds.X < 0)
						start.X += screen.Bounds.X;

					if (screen.Bounds.Y < 0)
						start.Y += screen.Bounds.Y;
				}

				// change the signs if the starting point
				start.X = Math.Abs((int)((double)start.X));
				start.Y = Math.Abs((int)((double)start.Y));

				foreach (KeyValuePair<string, ScreenSettings> screenSettings in ScreenSettingsCollection.Current)
				{
					Screen screen = zWallpaperUtility.GetScreen(screenSettings.Value);

					if (screen == null)
						continue;

					Rectangle bounds = screen.Bounds;

					// set offset
					bounds.Offset(start);

					// get and place image
					graphics.DrawImage(DesktopImage(screenSettings.Value), bounds);
				}

				// since windows repeats textures starting from the upper left and our image was built from the center,
				//	we need to translate it to compensate for the offset
				if (start.X != 0  || start.Y != 0)
				{
					// create a texture brush with our composite image and simply redraw the image, textured, at the starting offset
					TextureBrush tb = new TextureBrush(wallpaper, WrapMode.Tile);
					tb.TranslateTransform(start.X * -1, start.Y * -1);

					graphics.FillRegion(tb, new Region(new Rectangle(new Point(0, 0), screenSize)));
				}
			}
			catch (Exception ex) { error = ex; }

			return wallpaper;
		}
		#endregion

		#region Show Error
		public static void ShowError(Exception exception)
		{
			if (exception == null)
				return;
#if DEBUG
			MessageBox.Show(exception.ToString());
#else
				MessageBox.Show(exception.Message);
#endif
		}
		#endregion
	}
}