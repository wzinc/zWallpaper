using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace zWallpaper
{
	#region WallPaperStyle
	/// <summary>
	/// WallPaperStyle types
	/// </summary>
	public enum WallPaperStyle
	{
		/// <summary>
		/// Fit to Screen
		/// </summary>
		FitToScreen,

		/// <summary>
		/// Fill Screen
		/// </summary>
		FillScreen,

		/// <summary>
		/// Stretch to Fill Screen
		/// </summary>
		StretchToFillScreen,

		/// <summary>
		/// Center
		/// </summary>
		Center,

		/// <summary>
		/// Tile
		/// </summary>
		Tile
	}
	#endregion

	/// <summary>
	/// A container for screen settings.
	/// </summary>
	[Serializable]
	public class ScreenSettings
	{
		#region Properties
		/// <summary>
		/// Gets or sets the background color.
		/// </summary>
		public Color BackgroundColor { get; set; }

		/// <summary>
		/// Gets the screen id.
		/// </summary>
		public string ScreenID { get; internal set; }

		/// <summary>
		/// Gets the screen checksum.
		/// </summary>
		public string Checksum { get; internal set; }

		/// <summary>
		/// Gets or sets the wallpaper path.
		/// </summary>
		public string WallpaperPath { get; set; }

		/// <summary>
		/// Gets or sets the wallpaper style.
		/// </summary>
		public WallPaperStyle Style { get; set; }
		#endregion

		#region Serialization
		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns>serialized instance</returns>
		public string Serialize()
		{
			using (MemoryStream output = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(output, this);

				return Convert.ToBase64String(output.GetBuffer());
			}
		}

		/// <summary>
		/// Deserializes a ScreenSettings instance.
		/// </summary>
		/// <param name="serializedObject">serialized instance</param>
		/// <returns>deserialized instance</returns>
		public static ScreenSettings Deserialize(string serializedObject)
		{
			using (MemoryStream input = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(input))
			{
				byte[] objectArray = Convert.FromBase64String(serializedObject);
				input.Write(objectArray, 0, objectArray.Length);
				writer.Write(serializedObject);
				input.Seek(0, SeekOrigin.Begin);

				BinaryFormatter binaryFormatter = new BinaryFormatter();

				return (ScreenSettings)binaryFormatter.Deserialize(input);
			}
		}
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Creates a new ScreenSettings instance.
		/// </summary>
		/// <param name="screenID">screen id</param>
		/// <param name="checksum">checksum</param>
		/// <param name="wallpaperPath">wallpaper path</param>
		/// <param name="backgroundColor">background color</param>
		/// <param name="style">style</param>
		public ScreenSettings(string screenID, string checksum, string wallpaperPath, Color backgroundColor,
			WallPaperStyle style)
		{
			ScreenID = screenID;
			Checksum = checksum;

			BackgroundColor = backgroundColor;
			WallpaperPath = wallpaperPath;
			Style = style;
		}
		#endregion
	}
}