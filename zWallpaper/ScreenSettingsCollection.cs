using System;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows.Forms;
using global::BZFramework.Util;
using System.Runtime.Serialization;
using zWallpaper.Properties;

namespace zWallpaper
{
	/// <summary>
	/// A collection of ScreenSettings instances.
	/// </summary>
	[Serializable]
	public class ScreenSettingsCollection : Dictionary<string, ScreenSettings>
	{
		#region Members
		private static ScreenSettingsCollection _current;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets collection members.
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>ScreenSettings instance</returns>
		public new ScreenSettings this[string key]
		{
			get { return ContainsKey(key) ? (ScreenSettings)base[key] : null; }
			set
			{
				if (!ContainsKey(key))
					Add(key, value);
				else
					base[key] = value;
			}
		}
		#endregion

		#region Constructor(s)
		static ScreenSettingsCollection()
		{
			if (string.IsNullOrEmpty(Settings.Default.ScreenSettingsCache))
				InitializeScreenSettings();
		}

		private static void InitializeScreenSettings()
		{
			Current.Clear();

			// build screen settings instances from Screen.AllScreens
			foreach (Screen screen in Screen.AllScreens)
				Current[screen.DeviceName] = new ScreenSettings(screen.DeviceName, screen.ToString(), null, Color.Black, WallPaperStyle.Center);
		}
		#endregion

		#region Add Methods
		/// <summary>
		/// Adds a ScreenSettings object to the collection with an automatic key of ScreenSettings.ScreenID.
		/// </summary>
		/// <param name="value">ScreenSettings object</param>
		public void Add(ScreenSettings value)
		{
			Add(value.ScreenID, value);
		}

		/// <summary>
		/// Adds a ScreenSettings object to the collection.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="value">ScreenSettings object</param>
		public new void Add(string key, ScreenSettings value)
		{
			if (ContainsKey(key))
				this[key] = value;
			else
				base.Add(key, value);
		}
		#endregion

		#region Serialization
		/// <summary>
		/// Serializes this instance.
		/// </summary>
		/// <returns>serialized instance</returns>
		public string Serialize()
		{
			SerializableValueList values = new SerializableValueList();

			foreach (string key in this.Keys)
				values[key] = this[key].Serialize();

			return values.ToString();
		}

		/// <summary>
		/// Deserializes a ScreenSettingsCollection instance.
		/// </summary>
		/// <param name="serializedObject">serialized instance</param>
		/// <returns>ScreenSettingsCollection instance</returns>
		public static ScreenSettingsCollection Deserialize(string serializedObject)
		{
			if (string.IsNullOrEmpty(serializedObject))
				return null;

			SerializableValueList values = SerializableValueList.Parse(serializedObject);

			ScreenSettingsCollection screenSettingsCollection = new ScreenSettingsCollection();

			foreach (string key in values.Keys)
				screenSettingsCollection.Add(key, ScreenSettings.Deserialize(values[key]));

			return screenSettingsCollection;
		}
		#endregion

		#region Current Access
		/// <summary>
		/// Gets the current ScreenSettingsCollection.
		/// </summary>
		public static ScreenSettingsCollection Current
		{
			get
			{
				return _current == null && (_current = Deserialize(Settings.Default.ScreenSettingsCache)) == null ? _current = new ScreenSettingsCollection() :
					_current;
			}
		}

		/// <summary>
		/// Serializes and saves the current ScreenSettingsCollection.
		/// </summary>
		public static void SaveSettings()
		{
			Settings.Default.ScreenSettingsCache = _current != null ? _current.Serialize() : null;
			Settings.Default.Save();
		}
		#endregion
	}
}