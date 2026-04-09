using BepInEx.Configuration;
using HFramework;

namespace HFrameworkLoader
{
	internal class Config
	{
		internal static Config Instance { get; set; } = new Config();

		private ConfigEntry<bool>? ReplaceOriginalScenes { get; set; }

		private ConfigEntry<bool>? DebugConditions { get; set; }

		internal void Init(ConfigFile conf)
		{
			var hfConfig = HFConfig.GetInstance();

			this.ReplaceOriginalScenes = conf.Bind<bool>(
				"General",
				"ReplaceOriginalScenes",
				true,
				"Whether the mod should replace the H-Scenes original processing with its own.\n"
				+ "If true, the mod scenes takes over the original ones, usually this is not noticeable, but allows modders to do more stuff. But it may break during game updates.\n"
				+ "If false, the original game process will be used.\n"
				+ "Most mods that expand the H System likely needs this set to True"
			);
			hfConfig.ReplaceOriginalScenes = this.ReplaceOriginalScenes.Value;
			this.ReplaceOriginalScenes.SettingChanged += (sender, eventArgs) =>
			{
				hfConfig.ReplaceOriginalScenes = this.ReplaceOriginalScenes.Value;
			};

			this.DebugConditions = conf.Bind<bool>(
				"Debug",
				"DebugConditions",
				false,
				"Whether to log debug information about conditions. Every time a sex check is triggered, details of each condition will be displayed.\n"
				+ "This is useful for debugging, but very noisy."
			);
			hfConfig.DebugConditions = this.DebugConditions.Value;
			this.DebugConditions.SettingChanged += (sender, eventArgs) =>
			{
				hfConfig.DebugConditions = this.DebugConditions.Value;
			};
		}
	}
}
