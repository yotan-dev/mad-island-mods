using System.Collections.Generic;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.AssWall
{
	public class AssWallSceneTracker
	{
		public delegate void UnlockInfo(GalleryChara player, GalleryChara girl, InventorySlot.Type wallType);

		public event UnlockInfo OnUnlock;

		private GalleryChara Player = null;
		
		private GalleryChara Girl = null;

		private bool DidToilet = false;

		private bool DidCreampie = false;

		private InventorySlot.Type WallType = InventorySlot.Type.None;

		public AssWallSceneTracker()
		{
			AssWallPatch.OnStart += this.OnStart;
			AssWallPatch.OnEnd += this.OnEnd;
			SexCountPatch.OnToilet += this.OnToiletCount;
			SexCountPatch.OnCreampie += this.OnCreampieCount;
		}

		private void OnStart(AssWallPatch.AssWallInfo info)
		{
			GalleryLogger.LogDebug($"AssWallSceneTracker#OnStart");
			if (this.Player != null || this.Girl != null) {
				GalleryLogger.LogError($"AssWallSceneTracker#OnStart: Already active. Man: {this.Player} / Girl: {this.Girl} -- Dropping previous scene");
			}

			this.Player = new GalleryChara(info.Player);
			this.Girl = new GalleryChara(info.Girl);
			this.DidToilet = false;
			this.DidCreampie = false;
			this.WallType = info.WallType;
		}

		public void OnToiletCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != this.Player?.OriginalChara || value.to != this.Girl?.OriginalChara) {
				// LoveLogger.LogDebug($"AssWallSceneTracker#OnToiletCount: From ({value.from?.charaName}) != man / To != girl ({value.to?.charaName})");
				return;
			}

			this.DidToilet = true;
		}

		public void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != this.Player?.OriginalChara || value.to != this.Girl?.OriginalChara) {
				// LoveLogger.LogDebug($"AssWallSceneTracker#OnCreampieCount: From ({value.from?.charaName}) != man / To != girl ({value.to?.charaName})");
				return;
			}

			this.DidCreampie = true;
		}

		private void OnEnd(AssWallPatch.AssWallInfo info)
		{
			if (this.Player?.OriginalChara != info.Player || this.Girl?.OriginalChara != info.Girl) {
				GalleryLogger.LogError($"AssWallSceneTracker#OnEnd: AssWall ended with different characters. Ignoring.");
				return;
			}

			if (this.DidToilet && this.DidCreampie) {
				this.OnUnlock?.Invoke(this.Player, this.Girl, this.WallType);
			} else {
				var desc = $"{this.Player} x {this.Girl} (WallType: {this.WallType})";
				GalleryLogger.LogDebug($"AssWallSceneTracker#OnEnd: 'DidToilet' ({this.DidToilet}) or 'DidCreampie' ({this.DidCreampie}) not set -- event NOT unlocked for {desc}");
			}
		}
	}
}