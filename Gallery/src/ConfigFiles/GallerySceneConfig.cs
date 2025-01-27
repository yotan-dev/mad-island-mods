using System.ComponentModel;
using System.Xml.Serialization;
using Gallery.GalleryScenes;

namespace Gallery.ConfigFiles
{
	public class GallerySceneConfig
	{
		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("dlc")]
		[DefaultValue(false)]
		public bool RequiresDLC = false;

		[XmlAttribute("minVersion")]
		[DefaultValue("")]
		public string MinVersion = "";

		[XmlArray("Actors"), XmlArrayItem("Actor")]
		public GalleryActor[] Actors;

		[XmlElement]
		public BaseController Controller;

	}
}
