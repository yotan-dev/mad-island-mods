using BepInEx.Configuration;

namespace EnhancedIsland
{
	public class PConfig
	{
		public static PConfig Instance { get; private set; } = new PConfig();

		// Not worth fighting with this error for configs, as they will all be initialized at program startup.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		public ConfigEntry<bool> EnableCraftColors { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		internal void Init(ConfigFile conf)
		{
			this.EnableCraftColors = conf.Bind<bool>(
				"CraftColors",
				"Enabled",
				true,
				"Enable Craft Colors enhancement?\n"
				+ "If true, when looking to craft items, missing requirements will be marked in red.\n"
				+ "If false, no changes are made to the game.\n"
			);
		}
	}
}
