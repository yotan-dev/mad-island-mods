using BepInEx.Configuration;

namespace EnhancedIsland.BetterWorkplaces
{
	public class PConfig
	{
		public static PConfig Instance = new();

		// Not worth fighting with this error for configs, as they will all be initialized at program startup.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		public ConfigEntry<bool> Enabled { get; private set; }
		
		public ConfigEntry<bool> LogBonus { get; private set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		internal void Init(ConfigFile conf)
		{
			this.Enabled = conf.Bind<bool>(
				"BetterWorkplaces",
				"Enabled",
				true,
				"Enable Better Workplaces enhancement?\n"
				+ "If true, some of NPC working process is changed to give better outcomes.\n"
				+ "If false, no changes are made to the game.\n"
			);
			this.LogBonus = conf.Bind<bool>("BetterWorkplaces", "LogBonuses", false, "Logs result numbers in BepInEx terminal");
		}
	}
}
