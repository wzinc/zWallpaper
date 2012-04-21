using System;
using System.Runtime.InteropServices;

namespace zWallpaper.Lib
{
	/// <summary>
	/// Class for external methods.
	/// </summary>
	public class Win32Interop
	{
		#region Bool
		/// <summary>
		/// Boolean enumeration.
		/// </summary>
		public enum Bool : int
		{
			/// <summary>
			/// False
			/// </summary>
			False = 0,

			/// <summary>
			/// True
			/// </summary>
			True
		}
		#endregion

		#region Point
		/// <summary>
		/// A Point struct for working with native libraries.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			#region Members
			/// <summary>
			/// X
			/// </summary>
			public Int32 X;

			/// <summary>
			/// Y
			/// </summary>
			public Int32 Y;
			#endregion

			#region Constructor(s)
			/// <summary>
			/// Creates a Point instance.
			/// </summary>
			/// <param name="x">x</param>
			/// <param name="y">y</param>
			public Point(Int32 x, Int32 y)
			{
				X = x;
				Y = y;
			}
			#endregion
		}
		#endregion

		#region Size
		/// <summary>
		/// A Size struct for working with native libraries.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Size
		{
			#region Members
			/// <summary>
			/// X
			/// </summary>
			public Int32 X;

			/// <summary>
			/// Y
			/// </summary>
			public Int32 Y;
			#endregion

			#region Constructor(s)
			/// <summary>
			/// Creates a Size instance.
			/// </summary>
			/// <param name="x">x</param>
			/// <param name="y">y</param>
			public Size(Int32 x, Int32 y)
			{
				X = x;
				Y = y;
			}
			#endregion
		}
		#endregion

		#region ARGB
		/// <summary>
		/// An ARGB struct for working with native libraries.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		struct ARGB
		{
			/// <summary>
			/// Blue value
			/// </summary>
			public byte Blue;

			/// <summary>
			/// Green value
			/// </summary>
			public byte Green;

			/// <summary>
			/// Red value
			/// </summary>
			public byte Red;

			/// <summary>
			/// Alpha value
			/// </summary>
			public byte Alpha;
		}
		#endregion

		#region Consts
		/// <summary>
		/// ULW_COLORKEY
		/// </summary>
		public const Int32 ULW_COLORKEY = 0x00000001;

		/// <summary>
		/// ULW_ALPHA
		/// </summary>
		public const Int32 ULW_ALPHA = 0x00000002;

		/// <summary>
		/// ULW_OPAQUE
		/// </summary>
		public const Int32 ULW_OPAQUE = 0x00000004;

		/// <summary>
		/// AC_SRC_OVER
		/// </summary>
		public const byte AC_SRC_OVER = 0x00;

		/// <summary>
		/// AC_SRC_ALPHA
		/// </summary>
		public const byte AC_SRC_ALPHA = 0x01;
		#endregion

		#region BLENDFUNCTION
		/// <summary>
		/// An BLENDFUNCTION struct for working with native libraries.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			/// <summary>
			/// Blend Op byte
			/// </summary>
			public byte BlendOp;

			/// <summary>
			/// Blend Flags byte
			/// </summary>
			public byte BlendFlags;

			/// <summary>
			/// Source Constant Alpha byte
			/// </summary>
			public byte SourceConstantAlpha;

			/// <summary>
			/// Alpha Format byte
			/// </summary>
			public byte AlphaFormat;
		}
		#endregion

		#region External Methods
		/// <summary>
		/// Updates a natively drawn image.
		/// </summary>
		/// <param name="hWind">window handle</param>
		/// <param name="hdcDat">hdc data handle</param>
		/// <param name="pptDat">start point</param>
		/// <param name="psize">draw size</param>
		/// <param name="hdcSrc">hdc src handle</param>
		/// <param name="pprSrc">source point</param>
		/// <param name="crKey">cr key</param>
		/// <param name="pblend">blend mode</param>
		/// <param name="dwFlags">drawing flags</param>
		/// <returns>a value indicating whether or not the drawing was updated</returns>
		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern Bool UpdateLayeredWindow(IntPtr hWind, IntPtr hdcDat, ref Point pptDat, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey,
			ref BLENDFUNCTION pblend, Int32 dwFlags);

		/// <summary>
		/// Gets a DC.
		/// </summary>
		/// <param name="hWind">window handle</param>
		/// <returns>DC handle</returns>
		[DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWind);

		/// <summary>
		/// Releases a DC.
		/// </summary>
		/// <param name="hWind">window handle</param>
		/// <param name="hDC">DC handle</param>
		/// <returns>release value</returns>
		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern int ReleaseDC(IntPtr hWind, IntPtr hDC);

		/// <summary>
		/// Creates a compatible DC.
		/// </summary>
		/// <param name="hDC">DC handle</param>
		/// <returns>DC handle</returns>
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		/// <summary>
		/// Deletes a DC.
		/// </summary>
		/// <param name="hDC">DC handle</param>
		/// <returns>a value indicating whether or not the DC was deleted</returns>
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern Bool DeleteDC(IntPtr hDC);

		/// <summary>
		/// Selectes an object.
		/// </summary>
		/// <param name="hDC">DC handle</param>
		/// <param name="hObject">object handle</param>
		/// <returns>object handle</returns>
		[DllImport("gdi32.dll", ExactSpelling = true)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		/// <summary>
		/// Deletes an object.
		/// </summary>
		/// <param name="hObject">object handle</param>
		/// <returns>a value indicating whether or not the object was deleted</returns>
		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern Bool DeleteObject(IntPtr hObject);
		#endregion
	}
}