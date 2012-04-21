using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zWallpaper.Properties;

namespace zWallpaper
{
	/// <summary>
	/// Controls screen settings for one display.
	/// </summary>
	public partial class Display : Form
	{
		#region Members
		private List<Display> _displays;
		private bool _startupWindow;
		private Image _preview;
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether or not this window is for the primary display.
		/// </summary>
		public bool IsPrimaryDisplay
		{
			get { return ScreenSettings.ScreenID == Screen.PrimaryScreen.DeviceName; }
		}

		/// <summary>
		/// Gets the ScreenSettings instance associated with this window.
		/// </summary>
		public ScreenSettings ScreenSettings { get; private set; }
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Creates a Display instance.
		/// </summary>
		/// <param name="settings">ScreenSettings object</param>
		/// <param name="startupWindow"></param>
		public Display(ScreenSettings settings, bool startupWindow)
		{
			ScreenSettings = settings;
			_startupWindow = startupWindow;
			InitializeComponent();

			Paint += new PaintEventHandler(Display_Paint);
		}
		#endregion

		#region Events
		/// <summary>
		/// Shows the window and recreates child windows.
		/// </summary>
		public void ReShow()
		{
			OnShown(null);
			Show();
		}

		/// <summary>
		/// Sets control values and creates child windows.
		/// </summary>
		/// <param name="e">EventArgs object</param>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			// set control values
			backgroundColorDialog.Color = ScreenSettings.BackgroundColor;

			List<string> filter = new List<string>();
			filter.Add("All Images|*.bmp;*.emf;*.gif;*.ico;*.jpg;*.jpeg;*.jfif;*.png;*.tif;*.tiff;*.wmf");
			filter.Add("Bitmap (*.bmp)|*.bmp");
			filter.Add("Enhanced Metafile (*.emf)|*.emf");
			filter.Add("Graphics Interchange Format (*.gif)|*.gif");
			filter.Add("Windows icon (*.ico)|*.ico");
			filter.Add("Joint Photographic Experts Group (*.jpg)|*.jpg;*.jpeg;*.jfif");
			filter.Add("Portable Network Graphics (*.png)|*.png");
			filter.Add("Tagged Image File Format (*.tif)|*.tif;*.tiff");
			filter.Add("Windows Metafile (*.wmf)|*.wmf");

			openImageFile.Filter = string.Join("|", filter.ToArray());

			if (!string.IsNullOrEmpty(ScreenSettings.WallpaperPath))
			{
				openImageFile.InitialDirectory = Path.GetFullPath(ScreenSettings.WallpaperPath);
				selectedImagePath.Text = ScreenSettings.WallpaperPath;
			}
			else
				selectedImagePath.Text = "None";

			selectStyle.SelectedIndexChanged -= selectStyle_SelectedIndexChanged;
			selectStyle.DataSource = StyleMenuItem.GenerateStyleMenu();
			selectStyle.SelectedIndexChanged += selectStyle_SelectedIndexChanged;
			selectStyle.SelectedValue = ScreenSettings.Style;
			
			// create windows for other displays
			_displays = new List<Display>();

			if (IsPrimaryDisplay)
			{
				foreach (string key in ScreenSettingsCollection.Current.Keys)
					if (key != ScreenSettings.ScreenID)
					{
						ScreenSettings screenSettings = ScreenSettingsCollection.Current[key];
						Screen screen = zWallpaperUtility.GetScreen(screenSettings);

						if (screen == null)
							continue;

						Display display = new Display(screenSettings, false);

						// remove their border to users can easily find the primary display
						display.FormBorderStyle = FormBorderStyle.None;

						// center window in associated display
						display.StartPosition = FormStartPosition.Manual;
						display.Left = screen.Bounds.Left + ((screen.Bounds.Width - Width) / 2);
						display.Top = screen.Bounds.Top + ((screen.Bounds.Height - Height) / 2);
						display.ShowInTaskbar = false;
						_displays.Add(display);
						display.Show(this);
					}

				// focus primary after opening children
				Focus();
			}

			_updatePreview();
		}

		void Display_Paint(object sender, PaintEventArgs e)
		{
			// add border to secondary display windows
			if (!IsPrimaryDisplay)
				e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
		}

		private void selectImageButton_Click(object sender, EventArgs e)
		{
			// show open dialog
			switch (openImageFile.ShowDialog(this))
			{
				case DialogResult.OK:
					ScreenSettings.WallpaperPath = selectedImagePath.Text = openImageFile.FileName;
					ScreenSettingsCollection.SaveSettings();

					pictureBox.Refresh();
					UpdateWallpaper();
					break;
			}
		}

		private void backgroundColorButton_Click(object sender, EventArgs e)
		{
			// show color picker
			switch (backgroundColorDialog.ShowDialog(this))
			{
				case DialogResult.OK:
					ScreenSettings.BackgroundColor = backgroundColorDialog.Color;
					ScreenSettingsCollection.SaveSettings();

					pictureBox.Refresh();
					UpdateWallpaper();
					break;
			}
		}

		private void selectStyle_SelectedIndexChanged(object sender, EventArgs e)
		{
			// set style
			if (ScreenSettings.Style != (WallPaperStyle)selectStyle.SelectedValue)
			{
				ScreenSettings.Style = (WallPaperStyle)selectStyle.SelectedValue;
				ScreenSettingsCollection.SaveSettings();

				// some styles will always cover the entire background;
				//	disable the background color picker for those cases
				switch (ScreenSettings.Style)
				{
					case WallPaperStyle.FitToScreen:
					case WallPaperStyle.Center:
						backgroundColorButton.Enabled = true;
						break;
					case WallPaperStyle.FillScreen:
					case WallPaperStyle.StretchToFillScreen:
					case WallPaperStyle.Tile:
						backgroundColorButton.Enabled = false;
						break;
				}

				pictureBox.Refresh();
				UpdateWallpaper();
			}
		}

		/// <summary>
		/// Close child windows and save settings.
		/// </summary>
		/// <param name="e">EventArgs object</param>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (Settings.Default.FirstRun)
			{
				zWallpaperContext.notifyIcon.ShowBalloonTip(5, "zWallpaper",
					"Remember, zWallpaper is still running. To change this behavior, simply uncheck \"Listen for screen changes.\"", ToolTipIcon.Info);
				Settings.Default.FirstRun = false;
			}

			// destroy other displays
			if (_displays != null)
				foreach (Display display in _displays)
				{
					display.Close();
					display.Dispose();
				}

			base.OnClosing(e);

			// stay alive or save & quit
			if (Settings.Default.StayAliveInSystemTray)
				GC.Collect();
			else
			{
				Settings.Default.Save();
				Application.Exit();
			}
		}

		private void clearImageButton_Click(object sender, EventArgs e)
		{
			// set no image
			if (!string.IsNullOrEmpty(ScreenSettings.WallpaperPath))
			{
				ScreenSettings.WallpaperPath = null;
				selectedImagePath.Text = "None";

				UpdateWallpaper();
			}
		}
		#endregion

		#region Utilities
		private void _updatePreview()
		{
			// get screen image and resize
			_preview = zWallpaperUtility.DesktopImage(ScreenSettings);
			Graphics graphics = Graphics.FromImage(_preview);
			Size imageSize = new Size(_preview.Width, _preview.Height);

			// get resize ratio
			float ratio = Math.Min((float)pictureBox.ClientRectangle.Height / (float)imageSize.Height, (float)pictureBox.ClientRectangle.Width /
				(float)imageSize.Width);

			// get new size
			Rectangle newImageBounds = new Rectangle(pictureBox.ClientRectangle.Width / 2 - (int)((float)(imageSize.Width / 2) * ratio),
				pictureBox.ClientRectangle.Height / 2 - (int)((float)(imageSize.Height / 2) * ratio), (int)((float)imageSize.Width * ratio), (int)((float)
				imageSize.Height * ratio));

			newImageBounds.Y += 1;
			newImageBounds.Height -= 2;

			Image fullPreview = new Bitmap(pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height);
			graphics = Graphics.FromImage(fullPreview);

			graphics.DrawImage(_preview, newImageBounds);
			graphics.DrawRectangle(Pens.Black, newImageBounds);

			// set to preview
			pictureBox.Image = fullPreview;
		}

		/// <summary>
		/// Updates the wallpaper for all screens.
		/// </summary>
		private void UpdateWallpaper()
		{
			Exception error;
			zWallpaperUtility.UpdateWallpaper(out error);
			_updatePreview();

			zWallpaperUtility.ShowError(error);
		}
		#endregion
	}
}