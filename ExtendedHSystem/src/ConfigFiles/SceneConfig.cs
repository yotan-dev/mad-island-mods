using System.Collections.Generic;

namespace ExtendedHSystem.ConfigFiles
{
	public class SceneConfig
	{
		public string Id { get; set; }

		public IList<ScenePerformerConfig> Performers { get; set; }

		public SceneConfig()
		{
			this.Performers = new List<ScenePerformerConfig>();
		}
	}
}
