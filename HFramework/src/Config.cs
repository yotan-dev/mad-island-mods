using BepInEx.Configuration;

namespace HFramework
{
	public class Config
	{
		public static Config Instance { get; private set; } = new Config();

		public ConfigEntry<bool> ReplaceOriginalScenes { get; private set; }
		
		public ConfigEntry<bool> DontStartInvalidSex { get; private set; }

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
			this.DontStartInvalidSex = conf.Bind<bool>(
				"General",
				"DontStartInvalidSex",
				true,
				"Whether NPCs should try to sex when there are no scenes for them.\n"
				+ "If true, NPCs will not try to have sex when there are no scenes for them.\n"
				+ "If false, the original behavior will be used. 2 NPCs may try to have sex but fail once they reach the bed due to no scene being available (mod author thinks it is a bug).\n"
			);
		}
	}
}
