using BepInEx.Configuration;

namespace YoUnnoficialPatches
{
	public class PConfig
	{
		public static PConfig Instance { get; private set; } = new PConfig();

		public ConfigEntry<bool> DontStartInvalidSex { get; private set; }
		
		public ConfigEntry<bool> FixMosaic { get; private set; }

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

			this.FixMosaic = conf.Bind<bool>(
				"General",
				"FixMosaic",
				true,
				"Tries to fix some censor/mosaic that the original game can't clean up properly. If true, it will fix them according to your censor configs."
			);
		}
	}
}
