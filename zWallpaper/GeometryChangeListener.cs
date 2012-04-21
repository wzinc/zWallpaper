using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

namespace zWallpaper
{
	/// <summary>
	/// Listens for changes in screen resolution, depth, and location.
	/// </summary>
	public class GeometryChangeListener
	{
		#region Members
		private BackgroundWorker _listener;
		private string _geometryChecksum;
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Creates a new GeometryChangeListener instance.
		/// </summary>
		public GeometryChangeListener()
		{
			_listener = new BackgroundWorker();
			_listener.WorkerSupportsCancellation = true;
			_listener.DoWork += new DoWorkEventHandler(_listener_DoWork);
			_listener.RunWorkerAsync();
		}
		#endregion

		#region Events / Stop / Utilities
		private void _listener_DoWork(object sender, DoWorkEventArgs e)
		{
			string testChecksum;

			while (!e.Cancel)
			{
				// test current configuration against last checksum
				if ((testChecksum = _getGeometryChecksum()) != _geometryChecksum)
				{
					Exception error;

					// set new configuration & update wallpaper
					_geometryChecksum = testChecksum;
					zWallpaperUtility.UpdateWallpaper(out error);

					zWallpaperUtility.ShowError(error);
				}

				// sleep X seconds
				Thread.Sleep(Properties.Settings.Default.ListenerTimeout * 1000);
			}
		}

		/// <summary>
		/// Stops listening.
		/// </summary>
		public void Stop()
		{
			_listener.CancelAsync();
		}

		/// <summary>
		/// Creates a string of all screens that's unique to the current configuration.
		/// </summary>
		/// <returns>geometry checksum</returns>
		private string _getGeometryChecksum()
		{
			StringBuilder geometryChecksum = new StringBuilder();

			foreach (Screen screen in Screen.AllScreens)
				geometryChecksum.Append(screen.Bounds.ToString());

			return geometryChecksum.ToString();
		}
		#endregion
	}
}