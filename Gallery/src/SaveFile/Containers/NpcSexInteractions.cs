namespace Gallery.SaveFile.Containers
{
	public class NpcSexInteractions : CharacterInteraction
	{
		public SexManager.SexCountState SexType { get; set; }
		public SexPlace.SexPlaceType PlaceType { get; set; }
		public int PlaceGrade { get; set; }

		public NpcSexInteractions() { }

		public NpcSexInteractions(
			GalleryChara npc1,
			GalleryChara npc2,
			SexManager.SexCountState sexType,
			SexPlace.SexPlaceType placeType,
			int placeGrade
		)
			: base(npc1, npc2) {
			this.SexType = sexType;
			this.PlaceType = placeType;
			this.PlaceGrade = placeGrade;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.SexType == (obj as NpcSexInteractions).SexType
				&& this.PlaceType == (obj as NpcSexInteractions).PlaceType
				&& this.PlaceGrade == (obj as NpcSexInteractions).PlaceGrade;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.SexType * 100
				+ (int) this.PlaceType * 1000
				+ (int) this.PlaceGrade * 10000;
		}
	}
}