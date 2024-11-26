using System;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.PlayerRaped
{
	public class PlayerRapedSceneTracker
	{
		public delegate void OnSceneInfo(GalleryChara player, GalleryChara rapist);

		public event OnSceneInfo OnUnlock;

		private GalleryChara player;
		private GalleryChara rapist;

		private bool DidRape;

		public PlayerRapedSceneTracker() {
			PlayerRapedPatch.OnStart += this.OnStart;
			PlayerRapedPatch.OnEnd += this.OnEnd;

			SexCountPatch.OnRape += this.OnRapeCount;
		}

		private void OnStart(PlayerRapedPatch.PlayerRapedInfo value)
		{
			GalleryLogger.LogDebug($"PlayerRapedSceneTracker#OnStart");
			if (this.player != null || this.rapist != null) {
				GalleryLogger.LogError($"PlayerRapedSceneTracker#OnStart: Already active. Man: {this.player} / Girl: {this.rapist} -- Dropping previous scene");
			}

			this.player = new GalleryChara(value.player);
			this.rapist = new GalleryChara(value.rapist);
			this.DidRape = false;
		}

		public void OnRapeCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != rapist?.OriginalChara || value.to != player?.OriginalChara) {
				// LoveLogger.LogDebug($"PlayerRapedSceneTracker#OnRapeCount: From ({value.from?.charaName}) != man / To != girl ({value.to?.charaName})");
				return;
			}

			this.DidRape = true;
		}

		private void OnEnd(PlayerRapedPatch.PlayerRapedInfo value)
		{
			GalleryLogger.LogDebug($"PlayerRapedSceneTracker#OnEnd");
			if (value.player != player?.OriginalChara || value.rapist != rapist?.OriginalChara) {
				GalleryLogger.SceneErrorToPlayer(
					"ManRapes",
					new Exception($"Unexpected man rapes without active characters. Man: {player} / Girl: {rapist}")
				);
				return;
			}

			if (this.DidRape) {
				this.OnUnlock?.Invoke(this.player, this.rapist);
			} else {
				GalleryLogger.LogDebug($"PlayerRapedSceneTracker: OnEnd: 'DidRape' not set -- event NOT unlocked for {rapist}");
			}

			this.player = null;
			this.rapist = null;
			this.DidRape = false;
		}
	}
}