using BepInEx.Configuration;

namespace YoUnnoficialPatches
{
	public class Config
	{
		public static Config Instance { get; private set; } = new Config();

		public ConfigEntry<bool> DontStartInvalidSex { get; private set; }

		internal void Init(ConfigFile conf)
		{
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
