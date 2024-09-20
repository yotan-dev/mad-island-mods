namespace Gallery.SaveFile.Containers
{
	public class ToiletInteractions : CharacterInteraction
	{
		public InventorySlot.Type ToiletType { get; set; }

		public int ToiletSize { get; set; }

		public ToiletInteractions() {}

		public ToiletInteractions(GalleryChara npc1, GalleryChara npc2, InventorySlot.Type type, int size)
			: base(npc1, npc2) {
			this.ToiletType = type;
			this.ToiletSize = size;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.ToiletType == (obj as ToiletInteractions).ToiletType
				&& this.ToiletSize == (obj as ToiletInteractions).ToiletSize;
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ (int) this.ToiletType * 10000
				+ (int) this.ToiletSize * 100;
		}
	}
}