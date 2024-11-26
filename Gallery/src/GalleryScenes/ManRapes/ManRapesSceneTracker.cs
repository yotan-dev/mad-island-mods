using System;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.ManRapes
{
	public class ManRapesSceneTracker
	{
		public delegate void OnSceneInfo(GalleryChara man, GalleryChara girl, bool IsFainted, bool IsPregnant);

		public event OnSceneInfo OnUnlock;

		private GalleryChara man;
		private GalleryChara girl;

		private bool DidRape;

		private bool DidCreampie;

		private bool IsFainted;

		private bool IsPregnant;

		public ManRapesSceneTracker() {
			ManRapesPatch.OnStart += this.OnStart;
			ManRapesPatch.OnEnd += this.OnEnd;

			SexCountPatch.OnCreampie += this.OnCreampieCount;
			SexCountPatch.OnRape += this.OnRapeCount;
		}

		private void OnStart(ManRapesPatch.ManRapesInfo value)
		{
			GalleryLogger.LogDebug($"ManRapesSceneTracker#OnStart");
			if (this.man != null || this.girl != null) {
				GalleryLogger.LogError($"ManRapesSceneTracker#OnStart: Already active. Man: {this.man} / Girl: {this.girl} -- Dropping previous scene");
			}

			this.IsFainted = value.girl.faint == 0;
			this.IsPregnant = value.girl.pregnant[PregnantIndex.Father] != PregnantIndex.None;
			this.man = new GalleryChara(value.man);
			this.girl = new GalleryChara(value.girl);
			this.DidRape = false;
			this.DidCreampie = false;
		}

		public void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != man?.OriginalChara || value.to != girl?.OriginalChara) {
				// LoveLogger.LogDebug($"ManRapesSceneTracker#OnCreampieCount: From ({value.from?.charaName}) != man / To != girl ({value.to?.charaName})");
				return;
			}

			this.DidCreampie = true;
		}

		public void OnRapeCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != man?.OriginalChara || value.to != girl?.OriginalChara) {
				// LoveLogger.LogDebug($"ManRapesSceneTracker#OnRapeCount: From ({value.from?.charaName}) != man / To != girl ({value.to?.charaName})");
				return;
			}

			this.DidRape = true;
		}

		private void OnEnd(ManRapesPatch.ManRapesInfo value)
		{
			GalleryLogger.LogDebug($"ManRapesSceneTracker#OnEnd");
			if (value.man != man?.OriginalChara || value.girl != girl?.OriginalChara) {
				GalleryLogger.SceneErrorToPlayer(
					"ManRapes",
					new Exception($"Unexpected man rapes without active characters. Man: {man} / Girl: {girl}")
				);
				return;
			}

			if (this.DidRape && this.DidCreampie) {
				this.OnUnlock?.Invoke(this.man, this.girl, this.IsFainted, this.IsPregnant);
			} else {
				GalleryLogger.LogDebug($"ManRapeScene: OnEnd: 'DidRape' or 'DidCreampie' not set -- event NOT unlocked for {girl}");
			}

			this.man = null;
			this.girl = null;
			this.DidRape = false;
			this.DidCreampie = false;
			this.IsFainted = false;
		}
	}
}