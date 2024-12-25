using System.Collections.Generic;

namespace ExtendedHSystem.ConfigFiles
{
	public class PerformersConfig
	{
		public IList<PerformerConfig> Performers { get; set; }

		public PerformersConfig()
		{
			this.Performers = new List<PerformerConfig>();
		}
	}
}
