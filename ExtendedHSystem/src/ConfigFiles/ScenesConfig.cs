using System.Collections.Generic;

namespace ExtendedHSystem.ConfigFiles
{
	public class ScenesConfig
	{
		public IList<SceneConfig> Scenes { get; set; }

		public ScenesConfig()
		{
			this.Scenes = new List<SceneConfig>();
		}
	}
}
