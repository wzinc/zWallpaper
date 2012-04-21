using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace zWallpaper
{
	/// <summary>
	/// Splash Screen shows logo on start-up.
	/// 
	/// Derived from Godinho Lopes's PerPixelAlphaForm.
	/// http://www.codeproject.com/KB/GDI-plus/perpxalpha_sharp.aspx
	/// </summary>
	public partial class Splash : Form
	{
		#region Members
		private static BackgroundWorker _worker;
		private delegate void _delegate();
		private Bitmap _bitmap;
		#endregion

		#region Properties
		/// <summary>
		/// Gets form parameters.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams paramaters = base.CreateParams;
				paramaters.ExStyle |= 0x00080000;
				return paramaters;
			}
		}
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Creates a Splash instance.
		/// </summary>
		public Splash()
		{
			ShowInTaskbar = false;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;

			// iamge & location
			_bitmap = zWallpaper.Properties.Resources.logo;
			Left = Screen.PrimaryScreen.Bounds.Width - _bitmap.Width - 25;
			Top = Screen.PrimaryScreen.Bounds.Height - _bitmap.Height - 25;
		}
		#endregion

		#region Utilities
		/// <summary>
		/// Shows the splash screen.
		/// </summary>
		static public void ShowSplash()
		{
			Splash splash = new Splash();

			_worker = new BackgroundWorker();
			_worker.DoWork += new DoWorkEventHandler(_worker_DoWork);

			_worker.RunWorkerAsync(splash);
		}

		private static void _worker_DoWork(object sender, DoWorkEventArgs e)
		{
			Splash splash = (Splash)e.Argument;
			splash.CreateHandle();
			splash.Invoke(new _delegate(splash.Show));
			GC.Collect();
		}
		#endregion

		#region Events
		/// <summary>
		/// Shows splash screen.
		/// </summary>
		/// <param name="e">EventArgs object</param>
		protected override void OnShown(EventArgs e)
		{
			if (_bitmap.PixelFormat != PixelFormat.Format32bppArgb)
				throw new ApplicationException("Bitmap must be 32-bit with alpha-channel.");

			IntPtr screenDc = Win32Interop.GetDC(IntPtr.Zero);
			IntPtr memDc = Win32Interop.CreateCompatibleDC(screenDc);
			IntPtr hBitmap = IntPtr.Zero;
			IntPtr oldBitmap = IntPtr.Zero;

			try
			{
				hBitmap = _bitmap.GetHbitmap(Color.FromArgb(0));
				oldBitmap = Win32Interop.SelectObject(memDc, hBitmap);

				// set size & location
				Win32Interop.Size size = new Win32Interop.Size(_bitmap.Width, _bitmap.Height);
				Win32Interop.Point pointSource = new Win32Interop.Point(0, 0);
				Win32Interop.Point top = new Win32Interop.Point(Left, Top);
				Win32Interop.BLENDFUNCTION blend = new Win32Interop.BLENDFUNCTION();
				blend.BlendOp = Win32Interop.AC_SRC_OVER;
				blend.BlendFlags = 0;
				blend.AlphaFormat = Win32Interop.AC_SRC_ALPHA;

				// fade-in
				for (blend.SourceConstantAlpha = 0; blend.SourceConstantAlpha < 255; blend.SourceConstantAlpha += 5)
				{
					Win32Interop.UpdateLayeredWindow(Handle, screenDc, ref top, ref size, memDc, ref pointSource, 0, ref blend, Win32Interop.ULW_ALPHA);
					Thread.Sleep(1);
				}

				// let people see the wonderfor logo
				Thread.Sleep(3000);

				// fade-out
				for (blend.SourceConstantAlpha = 255; blend.SourceConstantAlpha > 0; blend.SourceConstantAlpha -= 5)
				{
					Win32Interop.UpdateLayeredWindow(Handle, screenDc, ref top, ref size, memDc, ref pointSource, 0, ref blend, Win32Interop.ULW_ALPHA);
					Thread.Sleep(1);
				}

				Close();
			}
			finally
			{
				Win32Interop.ReleaseDC(IntPtr.Zero, screenDc);

				if (hBitmap != IntPtr.Zero)
				{
					Win32Interop.SelectObject(memDc, oldBitmap);
					Win32Interop.DeleteObject(hBitmap);
				}

				Win32Interop.DeleteDC(memDc);
			}
		}
		#endregion
	}
}