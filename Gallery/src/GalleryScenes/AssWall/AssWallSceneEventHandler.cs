using Gallery.SaveFile.Containers;
using ExtendedHSystem;
using System.Collections;
using ExtendedHSystem.Scenes;

namespace Gallery.GalleryScenes.AssWall
{
	public class AssWallSceneEventHandler : SceneEventHandler
	{
		private readonly GalleryChara Player = null;

		private readonly GalleryChara Girl = null;

		private readonly InventorySlot.Type WallType = InventorySlot.Type.None;

		private bool DidToilet = false;

		private bool DidCreampie = false;

		public AssWallSceneEventHandler(
			CommonStates player,
			CommonStates girl,
			InventorySlot.Type wallType
		) : base("yogallery_asswall_handler")
		{
			this.Player = new GalleryChara(player);
			this.Girl = new GalleryChara(girl);
			this.WallType = wallType;
		}

		public override IEnumerable OnToilet(CommonStates from, CommonStates to)
		{
			this.DidToilet = true;
			yield return null;
		}

		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			this.DidCreampie = true;
			yield return null;
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
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

			yield return null;
		}
	}
}