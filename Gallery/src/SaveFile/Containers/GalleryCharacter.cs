using System.Linq;
using System.Xml.Serialization;
using YotanModCore;
using Gallery.UI;

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
				&& this.IsPregnant == other.IsPregnant;
		}
	
		// override object.GetHashCode
		public override int GetHashCode()
		{
			return (this.IsPregnant ? 1 : 0) << 16 + this.Id << 8 + this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return $"{this.Name} (ID: {this.Id}, Pregnant: {this.IsPregnant})";
		}

		public bool UnlockCheck(int id, bool isPregnant) {
			if (this.Id != id) {
				return false;
			}

			return isPregnant == this.IsPregnant;
		}

		public bool UnlockCheck(GallerySceneInfo.SceneNpc sceneNpc) {
			return this.UnlockCheck(sceneNpc.NpcID, sceneNpc.Pregnant);
		}
	}
}