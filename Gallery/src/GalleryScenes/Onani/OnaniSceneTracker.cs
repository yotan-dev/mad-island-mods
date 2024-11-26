using System;
using System.Collections.Generic;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Onani
{
	public class OnaniSceneTracker
	{
		public delegate void UnlockInfo(GalleryChara npc, bool perfume);

		public event UnlockInfo OnUnlock;

		// Key is FriendID currently masturbating. value is the start sex count.
		private Dictionary<int, int> ActiveCharaCounts = new Dictionary<int, int>();
		
		public OnaniSceneTracker()
		{
			OnaniNpcPatch.OnStart += this.OnStart;
			OnaniNpcPatch.OnEnd += this.OnEnd;
		}

		private void OnStart(OnaniNpcPatch.OnaniNpcInfo info)
		{
			GalleryLogger.LogDebug($"OnaniSceneTracker#OnStart");
			if (info.Npc.sexInfo.Length < 3) {
				GalleryLogger.LogError($"OnaniSceneTracker#OnStart: Invalid sex info. {new GalleryChara(info.Npc)} -- ignoring");
				return;
			}

			if (this.ActiveCharaCounts.ContainsKey(info.Npc.friendID)) {
				GalleryLogger.LogError($"OnaniSceneTracker#OnStart: Character already active. {new GalleryChara(info.Npc)} -- Dropping previous scene");
				this.ActiveCharaCounts.Remove(info.Npc.friendID);
			}

			this.ActiveCharaCounts.Add(info.Npc.friendID, info.Npc.sexInfo[2]);
		}

		private void OnEnd(OnaniNpcPatch.OnaniNpcInfo info)
		{
			var chara = new GalleryChara(info.Npc);
			if (!this.ActiveCharaCounts.ContainsKey(info.Npc.friendID)) {
				GalleryLogger.LogError($"OnaniSceneTracker#OnEnd: Masturbation ended for a NPC who wasn't masturbating? {chara}");
				return;
			}

			var count = this.ActiveCharaCounts[info.Npc.friendID];
			if (info.Npc.sexInfo[2] > count) {
				this.OnUnlock(chara, info.Perfume);
			}  else {
				var desc = $"{chara} (Perfume: {info.Perfume})";
				GalleryLogger.LogDebug($"OnaniSceneTracker#OnEnd: Count did not increase ({count} -> {info.Npc.sexInfo[2]}) -- event NOT unlocked for {desc}");
			}

			this.ActiveCharaCounts.Remove(info.Npc.friendID);
		}
	}
}