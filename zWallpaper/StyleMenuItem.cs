using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zWallpaper
{
	/// <summary>
	/// Internal struct for creating the style menu with WallPaperStyle enum values.
	/// </summary>
	public struct StyleMenuItem
	{
		#region Properties
		/// <summary>
		/// Gets or sets the item text.
		/// </summary>
		public string Text { get; internal set; }

		/// <summary>
		/// Gets or sets the item value.
		/// </summary>
		public WallPaperStyle Value { get; internal set; }
		#endregion

		#region Constructor(s)
		/// <summary>
		/// Creates a new StyleMenuItem instance.
		/// </summary>
		/// <param name="text">item text</param>
		/// <param name="value">item value</param>
		public StyleMenuItem(string text, WallPaperStyle value)
			: this()
		{
			Text = text;
			Value = value;
		}
		#endregion

		#region GenerateStyleMenu
		/// <summary>
		/// Generates the style menu items.
		/// </summary>
		/// <returns>style menu items</returns>
		public static StyleMenuItem[] GenerateStyleMenu()
		{
			List<StyleMenuItem> items = new List<StyleMenuItem>();

			items.Add(new StyleMenuItem("Fit To Screen", WallPaperStyle.FitToScreen));
			items.Add(new StyleMenuItem("Fill Screen", WallPaperStyle.FillScreen));
			items.Add(new StyleMenuItem("Stretch To Fill Screen", WallPaperStyle.StretchToFillScreen));
			items.Add(new StyleMenuItem("Center", WallPaperStyle.Center));
			items.Add(new StyleMenuItem("Tile", WallPaperStyle.Tile));

			return items.ToArray();
		}
		#endregion
	}
}