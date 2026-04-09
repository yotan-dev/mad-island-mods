using System.Collections.Generic;
using System.Xml.Serialization;

namespace HFramework.ConfigFiles
{
	public class SceneConfig
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlArrayItem("Performer")]
		public List<ScenePerformerConfig> Performers { get; set; }

		public SceneConfig()
		{
			this.Performers = new List<ScenePerformerConfig>();
		}
	}
}
