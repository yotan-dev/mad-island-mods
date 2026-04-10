#nullable enable

using System;
using System.Xml.Serialization;
using HFramework.Performer;

namespace HFramework.ConfigFiles
{
	[XmlRoot(ElementName = "Performer")]
	public class PerformerConfig
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; } = "NO ID";

		[XmlAttribute(AttributeName = "minVersion")]
		public string? MinVersion { get; set; } = "0.0.0";

		[XmlAttribute(AttributeName = "dlc")]
		public bool DLC { get; set; } = false;

		[XmlArray(ElementName = "Scopes")]
		[XmlArrayItem(ElementName = "Scope")]
		public PerformerScope[] Scopes { get; set; } = new PerformerScope[0];

		public BasePrefabSelector? PrefabSelector { get; set; }

		[XmlArray(ElementName = "Actors")]
		[XmlArrayItem(ElementName = "Actor")]
		public ConfigActor[] Actors { get; set; } = new ConfigActor[0];

		[XmlArray(ElementName = "AnimationSets")]
		[XmlArrayItem(ElementName = "AnimationSet")]
		public AnimationSetXml[] AnimationSets { get; set; } = new AnimationSetXml[0];
	}

	[Serializable]
	public class AnimationSetXml
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; } = "Default";

		[XmlArray(ElementName = "Animations")]
		[XmlArrayItem(ElementName = "Animation")]
		public AnimationsConfig[] Animations { get; set; } = new AnimationsConfig[0];
	}
}
