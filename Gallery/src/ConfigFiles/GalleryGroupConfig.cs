using System.Xml.Serialization;

namespace Gallery.ConfigFiles
{
	public class GalleryGroupConfig
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlArray("Scenes")]
		[XmlArrayItem("Scene")]
		public GallerySceneConfig[] Scenes { get; set; }
	}
}
