using BepInEx.Configuration;

namespace ExtendedHSystem.Configs
{
	public class RequireForeplayConfig
	{
		public static RequireForeplayConfig Instance { get; private set; } = new RequireForeplayConfig();

		public ConfigEntry<bool> Enabled { get; private set; }

		public ConfigEntry<float> BarLevel { get; private set; }

		public ConfigEntry<bool> UseAgeModifier { get; private set; }
		
		public ConfigEntry<bool> UseRandomModifier { get; private set; }

		internal void Init(ConfigFile conf)
		{
			this.Enabled = conf.Bind<bool>(
				"RequireForeplay",
				"Enabled",
				false,
				"Enables 'Require Foreplay' mod.\n"
				+ "When enabled, player controlled scenes will require player to reach certain level in the bar before being able to 'Insert'\n"
				+ "otherwise, the NPC will refuse to continue and abort the scene."
			);

			this.BarLevel = conf.Bind<float>(
				"RequireForeplay",
				"BarLevel",
				0.3f,
				"How much the bar must be filled before player can 'Insert'.\n"
				+ "Range 0-1, 0 being empty, 1 being full."
				+ "Note that above 0.3f it will be very slow to fill up the bar."
			);

			this.UseAgeModifier = conf.Bind<bool>(
				"RequireForeplay",
				"UseAgeModifier",
				true,
				"When true, older NPCs will have a lower bar requirement.\n"
				+ "When false, they will have the same bar requirement of 'BarLevel'."
			);

			this.UseRandomModifier = conf.Bind<bool>(
				"RequireForeplay",
				"UseRandomModifier",
				true,
				"When true, NPCs get a random bar requirement that persists during the play session.\n"
				+ "Simulating different 'tastes'"
			);
		}
	}
}
