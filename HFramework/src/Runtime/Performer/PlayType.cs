using System.Xml.Serialization;

namespace HFramework.Performer
{
	public enum PlayType
	{
		[XmlEnum(Name = "None")]
		None,

		[XmlEnum(Name = "Loop")]
		Loop,

		[XmlEnum(Name = "Once")]
		Once,
	}
}
