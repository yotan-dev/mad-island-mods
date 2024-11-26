namespace Gallery.SaveFile.Containers
{
	public class CommonSexPlayerInteraction : CharacterInteraction
	{
		public int SexType { get; set; }

		public int SpecialFlag { get; set; }

		public CommonSexPlayerInteraction() { }

		public CommonSexPlayerInteraction(GalleryChara player, GalleryChara npc, int sexType, int specialFlag)
			: base(player, npc) {
			this.SexType = sexType;
			this.SpecialFlag = specialFlag;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj)
				&& this.SexType.Equals((obj as CommonSexPlayerInteraction).SexType)
				&& this.SexType.Equals((obj as CommonSexPlayerInteraction).SpecialFlag);
		}
	
		public override int GetHashCode()
		{
			return base.GetHashCode()
				+ this.SexType * 100
				+ this.SpecialFlag;
		}
	}
}