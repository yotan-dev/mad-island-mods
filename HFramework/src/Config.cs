using BepInEx.Configuration;

namespace HFramework
{
	public class Config
	{
		public static Config Instance { get; private set; } = new Config();

		public ConfigEntry<bool> ReplaceOriginalScenes { get; private set; }

		public ConfigEntry<bool> DebugConditions { get; private set; }

		public ConfigEntry<bool> EnableLegacyScenes { get; private set; }

		internal void Init(ConfigFile conf)
		{
			this.ReplaceOriginalScenes = conf.Bind<bool>(
				"General",
				"ReplaceOriginalScenes",
				true,
				"Whether the mod should replace the H-Scenes original processing with its own.\n"
				+ "If true, the mod scenes takes over the original ones, usually this is not noticeable, but allows modders to do more stuff. But it may break during game updates.\n"
				+ "If false, the original game process will be used.\n"
				+ "Most mods that expand the H System likely needs this set to True"
			);

			this.DebugConditions = conf.Bind<bool>(
				"Debug",
				"DebugConditions",
				false,
				"Whether to log debug information about conditions. Every time a sex check is triggered, details of each condition will be displayed.\n"
				+ "This is useful for debugging, but very noisy."
			);

			this.EnableLegacyScenes = conf.Bind<bool>(
				"Compatibility",
				"EnableLegacyScenes",
				false,
				"Whether to enable the legacy scenes system to work together with SexScripts.\n"
				+ "If true, the legacy scenes will be enabled, and HFramework will randomly use SexScripts or legacy scenes on every interaction.\n"
				+ "If false, the legacy scenes will be disabled and HFramework will only use SexScripts (for those parts that were converted to SexScripts already).\n"
				+ "\n"
				+ "This setting is meant to ease the transition to the new SexScripts system.\n"
				+ "For modders trying the new SexScripts system, prefer to use this as false, as it will disable the old scenes system and you can see your work in action.\n"
				+ "For end users that still has a dependency on the legacy system, you can set this to true so it continues working."
			);
		}
	}
}
