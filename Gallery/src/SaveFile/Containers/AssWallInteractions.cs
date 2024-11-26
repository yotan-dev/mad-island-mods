namespace Gallery.SaveFile.Containers
{
	public class AssWallInteractions : CharacterInteraction
	{
		public InventorySlot.Type WallType { get; set; }

		public AssWallInteractions() {}

		public AssWallInteractions(GalleryChara npc1, GalleryChara npc2, InventorySlot.Type type)
			: base(npc1, npc2) {
			this.WallType = type;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.WallType == (obj as AssWallInteractions).WallType;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.WallType * 100;
		}
	}
}