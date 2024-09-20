using System;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Daruma
{
	public class DarumaSceneTracker
	{
		public delegate void UnlockInfo(GalleryChara player, GalleryChara girl);

		public event UnlockInfo OnUnlock;

		private GalleryChara Player = null;
		
		private GalleryChara Girl = null;

		private bool DidRape = false;

		private bool DidBust = false;

		public DarumaSceneTracker()
		{
			DarumaSexPatch.OnStart += this.OnStart;
			DarumaSexPatch.OnEnd += this.OnEnd;
			DarumaSexPatch.OnBust += this.OnBust;
			SexCountPatch.OnRape += this.OnRapecount;
		}

		private void Clear()
		{
			this.Player = null;
			this.Girl = null;
			this.DidRape = false;
			this.DidBust = false;
		}

		private void OnStart(DarumaSexPatch.DarumaInfo info)
		{
			GalleryLogger.LogDebug($"DarumaSceneTracker#OnStart");
			if (this.Player != null || this.Girl != null)
				GalleryLogger.LogError($"DarumaSceneTracker#OnStart: Already active. Man: {this.Player} / Girl: {this.Girl} -- Dropping previous scene");
			
			this.Clear();

			this.Player = new GalleryChara(info.Player);
			this.Girl = new GalleryChara(info.Girl);
		}

		private void OnRapecount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != this.Player?.OriginalChara || value.to != this.Girl?.OriginalChara) {
				// LoveLogger.LogDebug($"DarumaSceneTracker#OnRapeCount: From ({value.from?.charaName}) != man / To != girl ({value.to?.charaName})");
				return;
			}

			this.DidRape = true;
		}

		private void OnBust(DarumaSexPatch.DarumaInfo info)
		{
			if (info.Player != this.Player?.OriginalChara || info.Girl != this.Girl?.OriginalChara) {
				// GalleryLogger.LogDebug($"DarumaSceneTracker#OnBust: From ({info.Player?.charaName}) != {this.Player.Name} / To != girl ({info.Girl?.charaName} / {this.Girl.Name})");
				return;
			}

			this.DidBust = true;
		}

		private void OnEnd(DarumaSexPatch.DarumaInfo info)
		{
			if (this.Player?.OriginalChara != info.Player || this.Girl?.OriginalChara != info.Girl) {
				GalleryLogger.LogError($"DarumaSceneTracker#OnEnd: AssWall ended with different characters. Ignoring.");
				return;
			}

			if (this.DidRape && this.DidBust) {
				this.OnUnlock?.Invoke(this.Player, this.Girl);
			} else {
				var desc = $"{this.Player} x {this.Girl}";
				GalleryLogger.LogDebug($"DarumaSceneTracker#OnEnd: 'DidRape' ({this.DidRape}) or 'DidBust' ({this.DidBust}) not set -- event NOT unlocked for {desc}");
			}

			this.Clear();
		}
	}
}