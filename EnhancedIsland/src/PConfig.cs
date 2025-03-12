using BepInEx.Configuration;

namespace EnhancedIsland
{
	public class PConfig
	{
		public static PConfig Instance { get; private set; } = new PConfig();

		// Not worth fighting with this error for configs, as they will all be initialized at program startup.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		public BetterWorkplaces.PConfig BetterWorkplaces { get { return EnhancedIsland.BetterWorkplaces.PConfig.Instance; } }

		public ConfigEntry<bool> EnableDisassembleItems { get; private set; }

		public ConfigEntry<bool> EnableIncreaseZoom { get; private set; }

		public ConfigEntry<bool> EnableRequirementChecker { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		internal void Init(ConfigFile conf)
		{
			this.BetterWorkplaces.Init(conf);

			this.EnableDisassembleItems = conf.Bind<bool>(
				"DisassembleItems",
				"Enabled",
				true,
				"Allow player to disassemble items?\n"
				+ "If true, player may pess T to break items back into their crafting materials.\n"
				+ "If false, no changes are made to the game.\n"
			);

			this.EnableIncreaseZoom = conf.Bind<bool>(
				"IncreaseZoom",
				"Enabled",
				true,
				"Increase zoom out limit?\n"
				+ "If true, you can zoom out much farther away (Fog still applies).\n"
				+ "If false, no changes are made to the game.\n"
			);

			this.EnableRequirementChecker = conf.Bind<bool>(
				"RequirementChecker",
				"Enabled",
				true,
				"Enable Requirement Checker enhancement?\n"
				+ "If true, when looking to craft items, missing requirements will be marked in red.\n"
				+ "If false, no changes are made to the game.\n"
			);
		}
	}
}
