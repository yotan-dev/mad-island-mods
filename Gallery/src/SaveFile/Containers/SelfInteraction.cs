namespace Gallery.SaveFile.Containers
{
	public class SelfInteraction
	{
		public string PerformerId { get; set; }

		public GalleryChara Character1 { get; set; }

		public SelfInteraction() {}

		public SelfInteraction(string performerId, GalleryChara npc1) {
			this.PerformerId = performerId;
			this.Character1 = npc1;
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			var other = (SelfInteraction) obj;
		
			return this.Character1.Equals(other.Character1);
		}
	
		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.Character1.GetHashCode() << 16;
		}
	}
}
