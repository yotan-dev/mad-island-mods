using BepInEx.Configuration;
using UnityEngine;

namespace EnhancedIsland.RotateObject
{
	public class PConfig
	{
		public static PConfig Instance { get; private set; } = new PConfig();

		#nullable disable // Not worth fighting with nullable here, since we need to wait BepInEx startup

		public ConfigEntry<bool> Enabled { get; private set; }

		public ConfigEntry<KeyboardShortcut> Key { get; private set; }

		#nullable restore // Restore nullable check

		internal void Init(ConfigFile conf)
		{
			this.Enabled = conf.Bind<bool>(
				"RotateObject",
				"Enabled",
				true,
				"Enable Rotate Object enhancement?\n"
				+ "When true, you can rotate objects while placing them by pressing [Key] below.\n"
				+ "When false, no changes are made to the game."
			);

			this.Key = conf.Bind<KeyboardShortcut>(
				"RotateObject",
				"Key",
				new KeyboardShortcut(KeyCode.C),
				"Key to rotate object"
			);
		}
	}
}
