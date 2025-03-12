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

		public ConfigEntry<bool> EnableItemColorInSlot { get; private set; }

		public NpcStats.Config.PConfig NpcStats { get { return EnhancedIsland.NpcStats.Config.PConfig.Instance; } }

		public ConfigEntry<bool> EnableRequirementChecker { get; private set; }
		
		public ConfigEntry<bool> EnableStackNearby { get; private set; }

		public ConfigEntry<bool> EnableWarpBody { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		internal void Init(ConfigFile conf)
		{
			this.BetterWorkplaces.Init(conf);
			this.NpcStats.Init(conf);

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

			this.EnableItemColorInSlot = conf.Bind<bool>(
				"ItemColorInSlot",
				"Enabled",
				true,
				"Show bubble with item color on item slots (inventory/equipment/etc)?\n"
				+ "If true, you will see this visual indicator on those places.\n"
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

			this.EnableStackNearby = conf.Bind<bool>(
				"StackNearby",
				"Enabled",
				true,
				"Enable Stack Nearby enhancement?\n"
				+ "If true, you may press V to stack items from inventory into nearby containers.\n"
				+ "If false, no changes are made to the game.\n"
			);

			this.EnableWarpBody = conf.Bind<bool>(
				"WarpBody",
				"Enabled",
				true,
				"Enable Warp Body enhancement?\n"
				+ "If true, you may press P to warp the selected body to the closest \"garbage\" area.\n"
				+ "If false, no changes are made to the game.\n"
			);
		}
	}
}
