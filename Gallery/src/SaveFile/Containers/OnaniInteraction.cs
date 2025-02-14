namespace Gallery.SaveFile.Containers
{
	public class OnaniInteraction : SelfInteraction
	{
		public bool Perfume { get; set; }

		public OnaniInteraction() {}

		public OnaniInteraction(string performerId, GalleryChara npc, bool perfume)
			: base(performerId, npc) {
			this.Perfume = perfume;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.Perfume == (obj as OnaniInteraction).Perfume;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (this.Perfume ? 1 : 0);
		}
	}
}
