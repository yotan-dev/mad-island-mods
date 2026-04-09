using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HFramework.ConfigFiles
{
	[Serializable]
	public class PerformersConfig
	{
		[XmlArrayItem("Performer")]
		public List<PerformerConfig> Performers { get; set; }

		public PerformersConfig()
		{
			this.Performers = new List<PerformerConfig>();
		}
	}
}
