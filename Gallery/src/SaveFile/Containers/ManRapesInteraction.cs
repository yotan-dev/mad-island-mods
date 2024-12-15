namespace Gallery.SaveFile.Containers
{
	public class ManRapesInteraction : CharacterInteraction
	{
		public ManRapesInteraction() {}

		public ManRapesInteraction(GalleryChara npc1, GalleryChara npc2)
			: base(npc1, npc2) {
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}