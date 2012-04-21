using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zWallpaper.Properties;

namespace zWallpaper
{
	/// <summary>
	/// Application context for zWallpaper. Since we can exist only in the taskbar, we need a context, not just a window.
	/// </summary>
	public class zWallpaperContext : ApplicationContext
	{
		#region Static Members
		/// <summary>
		/// TeskBar icon
		/// </summary>
		public static NotifyIcon notifyIcon = new NotifyIcon();

		/// <summary>
		/// Listens for screen resolution, depth, and location changes.
		/// </summary>
		private static GeometryChangeListener listener;

		/// <summary>
		/// Primary display window.
		/// </summary>
		private static Display primaryDisplay;
		#endregion

		#region Members
		private AboutBox aboutBox;
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Creates a zWallpaperContext instance.
		/// </summary>
		public zWallpaperContext()
		{
			Splash.ShowSplash();

			notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
			notifyIcon.Icon = Resources.icon1;
			notifyIcon.Text = "zWallpaper";
			notifyIcon.Visible = true;

			ContextMenuStrip notifyMenu = new ContextMenuStrip();
			notifyMenu.Items.Add(new ToolStripMenuItem("Settings...", null, new EventHandler(notifyIcon_DoubleClick)));
			notifyMenu.Items.Add(new ToolStripSeparator());
			notifyMenu.Items.Add(new ToolStripMenuItem("About zWallpaper...", null, new EventHandler(About_Click)));
			notifyMenu.Items.Add(new ToolStripMenuItem("Listen for Screen Changes", null, new EventHandler(ListenForScreenChanges_Click)));
			notifyMenu.Items.Add(new ToolStripMenuItem("Run at Start-up", null, new EventHandler(RunAtStartup_Click)));
			notifyMenu.Items.Add(new ToolStripSeparator());
			notifyMenu.Items.Add(new ToolStripMenuItem("Exit", null, new EventHandler(Exit_Click)));

			if (Settings.Default.StayAliveInSystemTray)
				notifyMenu.Items[3].Image = Resources.selected;

			if (zWallpaperUtility.RunAtStartup)
				notifyMenu.Items[4].Image = Resources.selected;

			notifyIcon.ContextMenuStrip = notifyMenu;

			if (Settings.Default.FirstRun)
				notifyIcon.ShowBalloonTip(5, "Welcome to zWallpaper",
					"zWallpaper stays active in your system tray so that your desktop background can be updated whenever you switch resolution.",
					ToolTipIcon.Info);

			listener = new GeometryChangeListener();

			if (Settings.Default.FirstRun || !Settings.Default.StayAliveInSystemTray)
				DisplayWindowVisible = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether or not to show the display window(s).
		/// </summary>
		public bool DisplayWindowVisible
		{
			get { return primaryDisplay != null && primaryDisplay.Visible; }
			set
			{
				// don't do anything if it's already the way it should be
				if (value == DisplayWindowVisible)
					return;

				if (value)
				{
					// show window
					primaryDisplay = new Display(ScreenSettingsCollection.Current[Screen.PrimaryScreen.DeviceName], true);
					primaryDisplay.Show();
				}
				else
				{
					// hide window
					primaryDisplay.Close();
					primaryDisplay.Dispose();
					primaryDisplay = null;

					// stay alive or save & quit
					if (Settings.Default.StayAliveInSystemTray)
						GC.Collect();
					else
					{
						Settings.Default.Save();
						Application.Exit();
					}
				}
			}
		}
		#endregion

		#region Events
		void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			// show / hide window on icon double-click
			if (primaryDisplay == null || primaryDisplay.IsDisposed)
				DisplayWindowVisible = true;
			else
				DisplayWindowVisible = false;
		}

		void ListenForScreenChanges_Click(object sender, EventArgs e)
		{
			// set stay-alive value
			((ToolStripMenuItem)sender).Image = (Settings.Default.StayAliveInSystemTray = !Settings.Default.StayAliveInSystemTray) ? Resources.selected : null;
			Settings.Default.Save();

			// if the display window is closed, close the app too
			if (!Settings.Default.StayAliveInSystemTray && (primaryDisplay == null || !primaryDisplay.Visible))
				Exit_Click(sender, e);
		}

		void RunAtStartup_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;

			// set start-up value
			if (item.Image != null)
			{
				zWallpaperUtility.RunAtStartup = false;
				item.Image = null;
			}
			else
			{
				zWallpaperUtility.RunAtStartup = true;
				item.Image = Resources.selected;
			}
		}

		void Exit_Click(object sender, EventArgs e)
		{
			// hide icon & stip listener
			notifyIcon.Visible = false;
			listener.Stop();

			// save settings & quit
			ScreenSettingsCollection.SaveSettings();
			Settings.Default.Save();
			Application.Exit();
		}

		void About_Click(object sender, EventArgs e)
		{
			// just focus if already shown
			if (aboutBox != null)
			{
				aboutBox.Focus();
				return;
			}

			// show about box
			aboutBox = new AboutBox();
			aboutBox.StartPosition = FormStartPosition.CenterScreen;

			aboutBox.ShowDialog();
			aboutBox.Dispose();
		}
		#endregion
	}
}