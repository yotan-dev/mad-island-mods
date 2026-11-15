using System.Collections.Generic;
using System.Xml.Serialization;

namespace EnhancedIsland.DisassembleItems
{
	[XmlRoot(ElementName = "DisassembleTable")]
	public class UserDisassembleTableConfig
	{
		[XmlElement("Entry")]
		public List<UserDisassembleEntry> Entries { get; set; } = new List<UserDisassembleEntry>();
	}

	public class UserDisassembleEntry
	{
		[XmlAttribute("id")]
		public string Id { get; set; } = "";

		[XmlElement("Material")]
		public List<UserDisassembleMaterial> Materials { get; set; } = new List<UserDisassembleMaterial>();
	}

	public class UserDisassembleMaterial
	{
		[XmlAttribute("item")]
		public string Item { get; set; } = "";

		[XmlAttribute("count")]
		public int Count { get; set; }
	}
}
