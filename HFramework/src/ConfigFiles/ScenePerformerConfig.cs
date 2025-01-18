using System.ComponentModel;
using System.Xml.Serialization;
using HFramework.Scenes.Conditionals;

namespace HFramework.ConfigFiles
{
	public class ScenePerformerConfig
	{
		[XmlAttribute("id")]
		public string Performer { get; set; }

		[XmlArray("StartConditions")]
		[XmlArrayItem("Condition")]
		[DefaultValue(null)]
		public BaseConditional[] StartConditions { get; set; } = [];
		
		[XmlArray("PerformConditions")]
		[XmlArrayItem("Condition")]
		[DefaultValue(null)]
		public BaseConditional[] PerformConditions { get; set; } = [];
	}
}
