using System.Xml.Serialization;
using YotanModCore;

namespace Gallery.SaveFile.Containers
{
	public class GalleryChara
	{
		[XmlAttribute("Id")]
		public int Id { get; set; } = -1;

		[XmlAttribute("Name")]
		public string Name { get; set; } = "null";

		[XmlAttribute("Pregnant")]
		public bool IsPregnant { get; set; } = false;

		[XmlAttribute("IsFainted")]
		public bool IsFainted { get; set; } = false;

		[XmlIgnore]
		public CommonStates OriginalChara = null;

		internal GalleryChara() {}

		public GalleryChara(int npcId) {
			this.Id = npcId;
			this.Name = CommonUtils.GetName(npcId);
		}

		public GalleryChara(CommonStates commonStates) {
			this.Id = commonStates.npcID;
			this.Name = CommonUtils.GetName(commonStates.npcID);
			this.IsPregnant = commonStates.pregnant[0] != -1;
			this.IsFainted = commonStates.faint == 0.0f;
			this.OriginalChara = commonStates;
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			GalleryChara other = (GalleryChara)obj;
		
			return this.Id == other.Id
				&& this.Name == other.Name
				&& this.IsPregnant == other.IsPregnant
				&& this.IsFainted == other.IsFainted;
		}
	
		// override object.GetHashCode
		public override int GetHashCode()
		{
			return (this.IsFainted ? 1 : 0) << 17 + (this.IsPregnant ? 1 : 0) << 16 + this.Id << 8 + this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return $"{this.Name} (ID: {this.Id}, Pregnant: {this.IsPregnant})";
		}
	}
}
