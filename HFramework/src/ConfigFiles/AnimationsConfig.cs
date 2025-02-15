using System.Xml.Serialization;
using HFramework.Performer;

namespace HFramework.ConfigFiles
{
	public class AnimationsConfig
	{
		[XmlAttribute("action")]
		public ActionType Action { get; set; }
		
		[XmlAttribute("play")]
		public PlayType Play { get; set; }
		
		[XmlAttribute("name")]
		public string Name { get; set; }
		
		[XmlAttribute("pose")]
		public int Pose { get; set; } = 1;
		
		public string[] Events { get; set; } = [];
		
		[XmlAttribute("changePose")]
		public bool ChangePose { get; set; } = true;
	}
}
