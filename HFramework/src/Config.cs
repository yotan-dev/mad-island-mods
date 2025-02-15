using BepInEx.Configuration;

namespace HFramework
{
	public class Config
	{
		public static Config Instance { get; private set; } = new Config();

		public ConfigEntry<bool> ReplaceOriginalScenes { get; private set; }

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
		}
	}
}
