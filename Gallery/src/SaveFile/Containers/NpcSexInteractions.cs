namespace Gallery.SaveFile.Containers
{
	public class NpcSexInteractions : CharacterInteraction
	{
		public SexPlace.SexPlaceType PlaceType { get; set; }
		public int PlaceGrade { get; set; }

		public NpcSexInteractions() { }

		public NpcSexInteractions(
			string performerId,
			GalleryChara npc1,
			GalleryChara npc2,
			SexPlace.SexPlaceType placeType,
			int placeGrade
		)
			: base(performerId, npc1, npc2) {
			this.PlaceType = placeType;
			this.PlaceGrade = placeGrade;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.PlaceType == (obj as NpcSexInteractions).PlaceType
				&& this.PlaceGrade == (obj as NpcSexInteractions).PlaceGrade;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.PlaceType * 1000
				+ (int) this.PlaceGrade * 10000;
		}
	}
}
