using System.Collections.Generic;

namespace ExtendedHSystem.ConfigFiles
{
	public class ScenePerformerConfig
	{
		public string Performer { get; set; }

		public List<ConditionsConfig> StartConditions { get; set; } = [];
		
		public List<ConditionsConfig> PerformConditions { get; set; } = [];
	}
}
