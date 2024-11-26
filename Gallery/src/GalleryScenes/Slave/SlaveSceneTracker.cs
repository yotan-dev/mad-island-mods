using System;
using Gallery.Patches;
using Gallery.SaveFile.Containers;
using YotanModCore;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.Slave
{
	public class SlaveSceneTracker
	{
		public delegate void UnlockInfo(GalleryChara player, GalleryChara girl);

		public event UnlockInfo OnUnlock;

		private GalleryChara Player = null;
		
		private GalleryChara Girl = null;

		private bool DidBust = false;

		public SlaveSceneTracker()
		{
			SlavePatch.OnStart += this.OnStart;
			SlavePatch.OnEnd += this.OnEnd;
			SlavePatch.OnBust += this.OnBust;
		}

		private void Clear()
		{
			this.Player = null;
			this.Girl = null;
			this.DidBust = false;
		}

		private int Object2Npc(string obj)
		{
			switch (obj) {
				case "slave_giant_01": return NpcID.Giant;
				case "slave_shino_01": return NpcID.Shino;
				case "slave_sally_01": return NpcID.Sally;
				default: throw new Exception($"New slave: '{obj}'");
			}
		}

		private void OnStart(string obj)
		{
			var npcId = this.Object2Npc(obj);
			GalleryLogger.LogDebug($"SlaveSceneTracker#OnStart");
			if (this.Player != null || this.Girl != null)
				GalleryLogger.LogError($"SlaveSceneTracker#OnStart: Already active. Man: {this.Player} / Girl: {this.Girl} -- Dropping previous scene");
			
			this.Clear();

			this.Player = new GalleryChara(NpcID.Man);
			this.Girl = new GalleryChara(npcId);
		}

		private void OnBust()
		{
			this.DidBust = true;
		}

		private void OnEnd(string obj)
		{
			var npcId = this.Object2Npc(obj);
			if (this.Girl?.Id != npcId) {
				GalleryLogger.LogError($"SlaveSceneTracker#OnEnd: Slave ended with different characters. Ignoring.");
				return;
			}

			if (this.DidBust) {
				this.OnUnlock?.Invoke(this.Player, this.Girl);
			} else {
				var desc = $"{this.Player} x {this.Girl}";
				GalleryLogger.LogDebug($"SlaveSceneTracker#OnEnd: 'DidBust' ({this.DidBust}) not set -- event NOT unlocked for {desc}");
			}

			this.Clear();
		}
	}
}