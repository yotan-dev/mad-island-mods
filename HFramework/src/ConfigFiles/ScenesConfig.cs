using System.Collections.Generic;

namespace HFramework.ConfigFiles
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
