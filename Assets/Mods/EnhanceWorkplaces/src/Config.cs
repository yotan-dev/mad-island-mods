using BepInEx.Configuration;

namespace EnhanceWorkplaces
{
	public class Config
	{
		public static Config Instance = new Config();

		public ConfigEntry<bool> LogBonus;

		public void Init(ConfigFile conf)
		{
			this.LogBonus = conf.Bind<bool>("General", "LogBonuses", false, "Logs result numbers in BepInEx terminal");
		}
	}
}