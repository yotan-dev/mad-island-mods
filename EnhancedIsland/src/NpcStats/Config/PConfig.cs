using BepInEx.Configuration;

namespace EnhancedIsland.NpcStats.Config
{
	public class PConfig
	{
		public static PConfig Instance { get; private set; } = new PConfig();

		#nullable disable // Not worth fighting with nullable here, since we need to wait BepInEx startup

		public ConfigEntry<bool> Enabled { get; private set; }

		public ConfigEntry<DistributionMode> EnemiesDistribution { get; private set; }
		
		public ConfigEntry<bool> ExtraStrongEnemies { get; private set; }

		public ConfigEntry<DistributionMode> TamedNpcDistribution { get; private set; }

		public ConfigEntry<bool> ExtraStrongTamedNpc { get; private set; }

		public ConfigEntry<DistributionMode> NewbornDistribution { get; private set; }

		public ConfigEntry<bool> ExtraStrongNewborns { get; private set; }

		#nullable restore // Restore nullable check

		internal void Init(ConfigFile conf)
		{
			this.Enabled = conf.Bind<bool>(
				"NpcStats",
				"Enabled",
				true,
				"Enable NPC Stats enhancement?\n"
				+ "When true, NPC stats will be changed based on the other configs below."
				+ "When false, no changes are made to the game."
			);

			this.EnemiesDistribution = conf.Bind<DistributionMode>(
				"NpcStats.Stats",
				"EnemiesDistribution",
				DistributionMode.Random,
				"How are spawned enemies (natives in villages, etc) stats distributed?\n"
				+ "Note: ForceLevel1 and Keep won't work here."
			);
			this.ExtraStrongEnemies = conf.Bind<bool>(
				"NpcStats.Stats",
				"ExtraStrongEnemies",
				false,
				"Apply the default stats over the distribution for enemies? This will make enemies strength scale a lot as their level increases."
			);

			this.TamedNpcDistribution = conf.Bind<DistributionMode>(
				"NpcStats.Stats",
				"TamedNpcDistribution",
				DistributionMode.Random,
				"How are friendlied/tamed NPCs stats distributed?"
			);
			this.ExtraStrongTamedNpc = conf.Bind<bool>(
				"NpcStats.Stats",
				"ExtraStrongTamedNpc",
				false,
				"Apply the default stats over the distribution for Tamed NPCs? This will make NPCs that were tamed at a high level become much more stronger than leveled ones."
				+ "\nNote: This is ignored when TamedNpcDistribution is set to Keep."
			);

			this.NewbornDistribution = conf.Bind<DistributionMode>(
				"NpcStats.Stats",
				"NewbornDistribution",
				DistributionMode.Random,
				"How are newborns NPCs stats distributed?\n"
				+ "Note: Keep won't work here."
			);
			this.ExtraStrongNewborns = conf.Bind<bool>(
				"NpcStats.Stats",
				"ExtraStrongNewborns",
				false,
				"Apply the default stats distribution over the distribution for Newborn NPCs? This will make NPCs born at a high level become much more stronger than leveled ones."
			);
		}
	}
}
