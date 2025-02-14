namespace Gallery.SaveFile.Containers
{
	public class SexTypeInteraction<T> : CharacterInteraction
	{
		public T SexType { get; set; }

		public SexTypeInteraction() { }

		public SexTypeInteraction(string performerId, GalleryChara npc1, GalleryChara npc2, T sexType)
			: base(performerId, npc1, npc2) {
			this.SexType = sexType;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.SexType.Equals((obj as SexTypeInteraction<T>).SexType);
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.SexType.GetHashCode() * 100;
		}
	}
}
