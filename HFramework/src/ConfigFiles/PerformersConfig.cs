using System.Collections.Generic;

namespace HFramework.ConfigFiles
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
