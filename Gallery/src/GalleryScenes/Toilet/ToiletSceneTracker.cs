using System.Collections.Generic;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Toilet
{
	public class ToiletSceneTracker
	{
		private class RunningSceneContainer
		{
			public GalleryChara User;

			public GalleryChara Target;

			public int ToiletSize;

			public InventorySlot.Type ToiletType;

			public bool DidToilet;

			public bool DidCreampie;
		}

		public delegate void UnlockInfo(GalleryChara user, GalleryChara target, int toiletsize, InventorySlot.Type toiletType);

		public event UnlockInfo OnUnlock;

		private RunningSceneContainer RunningScene = null;

		public ToiletSceneTracker()
		{
			ToiletPatch.OnStart += this.OnStart;
			ToiletPatch.OnEnd += this.OnEnd;
			SexCountPatch.OnCreampie += this.OnCreampieCount;
			SexCountPatch.OnToilet += this.OnToiletCount;
		}

		private void OnStart(ToiletPatch.ToiletInfo info)
		{
			if (!CommonUtils.IsFriend(info.User)) {
				GalleryLogger.LogError($"ToiletSceneTracker#OnStart: Non-friend NPC using toilet. Unhandled case... {info.User.charaName}");
				return;
			}

			if (this.RunningScene != null)
				GalleryLogger.LogError($"ToiletSceneTracker#OnStart: scene already running. Resetting");

			this.RunningScene = new RunningSceneContainer()
			{
				User = new GalleryChara(info.User),
				Target = new GalleryChara(info.Target),
				ToiletSize = -1,
				ToiletType = InventorySlot.Type.None,
				DidToilet = false,
				DidCreampie = false,
			};
		}

		private void OnToiletCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (this.RunningScene == null)
				return;

			if (this.RunningScene.User?.OriginalChara != value.from || this.RunningScene.Target?.OriginalChara != value.to)
				return;
			
			RunningScene.DidToilet = true;
		}

		private void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (this.RunningScene == null)
				return;

			if (this.RunningScene.User?.OriginalChara != value.from || this.RunningScene.Target?.OriginalChara != value.to)
				return;
			
			RunningScene.DidCreampie = true;
		}

		private void OnEnd(ToiletPatch.ToiletInfo info)
		{
			if (this.RunningScene == null)
				return;

			if (this.RunningScene.User?.OriginalChara != info.User || this.RunningScene.Target?.OriginalChara != info.Target)
				return;

			if (RunningScene.DidToilet && RunningScene.DidCreampie) {
				this.OnUnlock?.Invoke(RunningScene.User, RunningScene.Target, RunningScene.ToiletSize, RunningScene.ToiletType);
			} else {
				var desc = $"{RunningScene.User} x {RunningScene.Target} (Size: {RunningScene.ToiletSize}, Type: {RunningScene.ToiletType})";
				GalleryLogger.LogDebug($"ToiletSceneTracker#OnEnd: 'DidCreampie'/'DidToilet' not set -- event NOT unlocked for {desc}");
			}
		}
	}
}