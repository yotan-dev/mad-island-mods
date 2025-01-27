using System.ComponentModel;
using System.Xml.Serialization;
using Gallery.SaveFile.Containers;
using YotanModCore;

namespace Gallery.ConfigFiles
{
	public class GalleryActor
	{
		/// <summary>
		/// Getter is needed or XmlSerializer fails
		/// </summary>
		[XmlAttribute("val")]
		public string Val { get => ""; set { this.ParseActor(value); } }

		[XmlIgnore]
		public int NpcId { get; private set; }

		[XmlIgnore]
		public string Constant { get; private set; }

		[DefaultValue(false)]
		[XmlAttribute("pregnant")]
		public bool Pregnant = false;

		[DefaultValue(false)]
		[XmlAttribute("fainted")]
		public bool Fainted = false;

		public GalleryActor() { }
		
		public GalleryActor(GalleryChara chara)
		{
			this.NpcId = chara.Id;
			this.Constant = "";
		}

		public GalleryActor(string val)
		{
			this.Val = val;
			this.ParseActor(val);
		}

		private void ParseActor(string val)
		{
			var parts = val.Split('#');

			this.NpcId = CommonUtils.ConstToId(parts[0]);
			this.Constant = parts[0];

			if (parts.Length >= 2 && int.TryParse(parts[1], out var result))
			{
				if (this.NpcId != result)
					PLogger.LogWarning($"Actor {val} has unmatching constant vs ID. Constant: {parts[0]} ({this.NpcId}) / ID: {result}");
			}
		}
	}
}
