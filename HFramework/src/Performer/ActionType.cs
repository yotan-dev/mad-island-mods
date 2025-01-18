using System.Xml.Serialization;

namespace HFramework.Performer
{
	public enum ActionType
	{
		[XmlEnum(Name = "None")]
		None,

		[XmlEnum(Name = "Battle")]
		Battle,

		[XmlEnum(Name = "Attack")]
		Attack,

		[XmlEnum(Name = "Defeat")]
		Defeat,

		[XmlEnum(Name = "StartIdle")]
		StartIdle,

		[XmlEnum(Name = "IdlePee")]
		IdlePee,

		[XmlEnum(Name = "Caress")]
		Caress,

		[XmlEnum(Name = "Insert")]
		Insert,

		[XmlEnum(Name = "InsertIdle")]
		InsertIdle,

		[XmlEnum(Name = "InsertPee")]
		InsertPee,

		[XmlEnum(Name = "Speed1")]
		Speed1,

		[XmlEnum(Name = "Speed2")]
		Speed2,

		[XmlEnum(Name = "Speed3")]
		Speed3,

		[XmlEnum(Name = "Finish")]
		Finish,

		[XmlEnum(Name = "FinishIdle")]
		FinishIdle,

		[XmlEnum(Name = "DeliveryIdle")]
		DeliveryIdle,

		[XmlEnum(Name = "DeliveryLoop")]
		DeliveryLoop,

		[XmlEnum(Name = "DeliveryEnd")]
		DeliveryEnd,
	}
}
