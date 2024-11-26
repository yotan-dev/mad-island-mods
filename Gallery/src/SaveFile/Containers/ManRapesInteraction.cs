namespace Gallery.SaveFile.Containers
{
	public class ManRapesInteraction : CharacterInteraction
	{
		public bool IsFainted { get; set; }

		public ManRapesInteraction() {}

		public ManRapesInteraction(GalleryChara npc1, GalleryChara npc2, bool isFainted)
			: base(npc1, npc2) {
			this.IsFainted = isFainted;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.IsFainted == (obj as ManRapesInteraction).IsFainted;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (this.IsFainted ? 1 : 0);
		}
	}
}