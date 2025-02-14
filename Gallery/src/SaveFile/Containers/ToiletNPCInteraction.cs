namespace Gallery.SaveFile.Containers
{
	public class ToiletNPCInteraction : CharacterInteraction
	{
		public SexPlace.SexPlaceType PlaceType { get; set; }

		public int ToiletGrade { get; set; }

		public ToiletNPCInteraction() {}

		public ToiletNPCInteraction(string performerId, GalleryChara npc1, GalleryChara npc2, SexPlace.SexPlaceType type, int grade)
			: base(performerId, npc1, npc2) {
			this.PlaceType = type;
			this.ToiletGrade = grade;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.PlaceType == (obj as ToiletNPCInteraction).PlaceType
				&& this.ToiletGrade == (obj as ToiletNPCInteraction).ToiletGrade;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.PlaceType * 10000
				+ (int) this.ToiletGrade * 100;
		}
	}
}
