using System.Xml.Serialization;

namespace HFramework.Performer
{
	public enum PerformerScope
	{
		[XmlEnum(Name = "None")]
		None,
		[XmlEnum(Name = "Battle")]
		Battle,
		[XmlEnum(Name = "Sex")]
		Sex,
		[XmlEnum(Name = "Delivery")]
		Delivery,
	}
}
