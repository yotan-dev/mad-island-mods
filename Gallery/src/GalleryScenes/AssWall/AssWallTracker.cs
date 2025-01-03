using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.AssWall
{
	public class AssWallTracker : BaseTracker
	{
		private readonly GalleryChara Player = null;

		private readonly GalleryChara Girl = null;

		private readonly InventorySlot.Type WallType = InventorySlot.Type.None;

		public AssWallTracker(
			CommonStates player,
			CommonStates girl,
			InventorySlot.Type wallType
		) : base()
		{
			this.Player = new GalleryChara(player);
			this.Girl = new GalleryChara(girl);
			this.WallType = wallType;
		}

		public override void End()
		{
			if (!this.DidToilet || !this.DidCreampie)
			{
				AssWallSceneManager.Instance.Unlock(this.Player, this.Girl, this.WallType);
			}
			else
			{
				var desc = $"{this.Player} x {this.Girl} (WallType: {this.WallType})";
				GalleryLogger.LogDebug($"AssWallSceneTracker#OnEnd: 'DidToilet' ({this.DidToilet}) or 'DidCreampie' ({this.DidCreampie}) not set -- event NOT unlocked for {desc}");
			}
		}
	}
}
