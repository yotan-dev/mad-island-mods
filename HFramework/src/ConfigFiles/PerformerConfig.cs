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

		[XmlArray(ElementName = "Scopes")]
		[XmlArrayItem(ElementName = "Scope")]
		public PerformerScope[] Scopes { get; set; } = [];

		public BasePrefabSelector? PrefabSelector { get; set; }

		[XmlArray(ElementName = "Actors")]
		[XmlArrayItem(ElementName = "Actor")]
		public ConfigActor[] Actors { get; set; } = [];

		[XmlArray(ElementName = "AnimationSets")]
		[XmlArrayItem(ElementName = "AnimationSet")]
		public AnimationSetXml[] AnimationSets { get; set; } = [];
	}

	[Serializable]
	public class AnimationSetXml
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; } = "Default";

		[XmlArray(ElementName = "Animations")]
		[XmlArrayItem(ElementName = "Animation")]
		public AnimationsConfig[] Animations { get; set; } = [];
	}
}
