using BepInEx.Configuration;

namespace Gallery
{
	public class Config
	{
		public static Config Instance = new Config();

		public ConfigEntry<bool> WriteLogs;

		public void Init(ConfigFile conf)
		{
			this.WriteLogs = conf.Bind<bool>("General", "WriteLogs", true, "Whether to write 'GalleryLogger.txt' file with everything related to gallery (useful for debugging/tracking)");
		}
	}
}
