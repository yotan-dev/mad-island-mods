using BepInEx.Configuration;
using HExtensions.RequireForeplay;

namespace HExtensions
{
	public class Config
	{
		public static Config Instance { get; private set; } = new Config();

		public RequireForeplayConfig RequireForeplay { get; private set; } = RequireForeplayConfig.Instance;

		public ConfigEntry<bool> EnableDickPainter { get; private set; }

		internal void Init(ConfigFile conf)
		{
			this.RequireForeplay.Init(conf);

			this.EnableDickPainter = conf.Bind<bool>(
				"DickPainter",
				"Enabled",
				true,
				"Enables DickPainter mod.\n"
				+ "This mod paints the male dick in red if the female is virgin or in pinky when using comdom.\n"
			);
		}
	}
}
