namespace Gallery.SaveFile.Containers
{
	public class StoryInteraction : CharacterInteraction
	{
		public StoryFlag Flag { get; set; }

		public StoryInteraction() { }

		public StoryInteraction(string performerId, GalleryChara npc1, GalleryChara npc2, StoryFlag flag)
			: base(performerId, npc1, npc2) {
			this.Flag = flag;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.Flag.Equals((obj as StoryInteraction).Flag);
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.Flag.GetHashCode() * 100;
		}
	}
}
