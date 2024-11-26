using YotanModCore;

namespace Gallery.SaveFile.Containers
{
	public class CharacterInteraction
	{
		public GalleryChara Character1 { get; set; }

		public GalleryChara Character2 { get; set; }

		public CharacterInteraction() {}

		public CharacterInteraction(GalleryChara npc1, GalleryChara npc2) {
			this.Character1 = npc1;
			this.Character2 = npc2;
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			var other = (CharacterInteraction) obj;
		
			return this.Character1.Equals(other.Character1)
				&& this.Character2.Equals(other.Character2);
		}
	
		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.Character1.GetHashCode() << 16 + this.Character2.GetHashCode();
		}
	}
}
