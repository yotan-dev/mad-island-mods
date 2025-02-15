using System;
using System.Xml.Serialization;

namespace Gallery.ConfigFiles
{
	[Serializable]
	[XmlRoot("GalleryGroups")]
	public class GalleryGroupsConfig
	{
		[XmlArray("Groups"), XmlArrayItem("Group")]
		public GalleryGroupConfig[] Groups { get; set; } = [];
	}
}
