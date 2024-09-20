using System;
using System.Collections.Generic;
using Gallery.Patches;
using Gallery.SaveFile.Containers;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.ManSleepRape
{
	public class ManSleepRapeSceneTracker
	{
		private class ModeInfo {
			public ManRapesSexType Mode;

			public bool DidRape;

			public bool DidCreampie;
		}

		public delegate void OnSceneInfo(GalleryChara man, GalleryChara girl, ManRapesSexType mode);

		public event OnSceneInfo OnUnlock;

		private GalleryChara man;
		private GalleryChara girl;

		private List<ModeInfo> Modes = new List<ModeInfo>();

		private ModeInfo CurrentMode = null;

		public ManSleepRapeSceneTracker() {
			ManRapesSleepPatch.OnStart += this.OnStart;
			ManRapesSleepPatch.OnSexTypeChange += this.OnSexTypeChange;
			ManRapesSleepPatch.OnEnd += this.OnEnd;

			SexCountPatch.OnCreampie += this.OnCreampieCount;
			SexCountPatch.OnRape += this.OnRapeCount;
		}

		private void OnStart(ManRapesSleepPatch.ManSleepRapesInfo value)
		{
			GalleryLogger.LogDebug($"ManSleepRapeSceneTracker#OnStart");
			if (this.man != null || this.girl != null) {
				GalleryLogger.LogError($"ManSleepRapeSceneTracker#OnStart: Already active. Man: {this.man} / Girl: {this.girl} -- Dropping previous scene");
			}

			this.man = new GalleryChara(value.man);
			this.girl = new GalleryChara(value.girl);
			this.Modes = new List<ModeInfo>();
			this.CurrentMode = null;
		}

		public void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (this.man?.OriginalChara != value.from || this.girl?.OriginalChara != value.to || this.CurrentMode == null) {
				return;
			}

			this.CurrentMode.DidCreampie = true;
		}

		public void OnRapeCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (this.man?.OriginalChara != value.from || this.girl?.OriginalChara != value.to || this.CurrentMode == null) {
				return;
			}

			this.CurrentMode.DidRape = true;
		}

		private void OnSexTypeChange(int state, int sexType)
		{
			if (this.man == null || this.girl == null) {
				GalleryLogger.LogDebug($"ManSleepRapeSceneTracker#OnSexTypeChange: Received sex type change while scene is not active?");
				return;
			}

			ManRapesSexType type;
			if (state == ManRapeSleepConst.StartRape) {
				type = ManRapesSexType.Rape;
			} else if (state == ManRapeSleepConst.StartDiscretlyRape) {
				if (sexType == 0) {
					type = ManRapesSexType.SleepPowder;
				} else {
					type = ManRapesSexType.DiscretlyRape;
				}
			} else {
				GalleryLogger.LogDebug($"ManSleepRapeSceneTracker#OnSexTypeChange: Can't determine type from state: {state} / sexType: {sexType}");
				return;
			}

			var newMode = this.Modes.Find((mode) => mode.Mode == type);
			if (newMode == null) {
				newMode = new ModeInfo();
				newMode.Mode = type;
				this.Modes.Add(newMode);
			}

			CurrentMode = newMode;
		}

		private void OnEnd(ManRapesSleepPatch.ManSleepRapesInfo value)
		{
			GalleryLogger.LogDebug($"ManSleepRapeSceneTracker#OnEnd");
			if (value.man != man?.OriginalChara || value.girl != girl?.OriginalChara) {
				GalleryLogger.SceneErrorToPlayer(
					"ManRapes",
					new Exception($"Unexpected sleep rapes without active characters. Man: {man} / Girl: {girl}")
				);
				return;
			}

			foreach (var mode in this.Modes) {
				if (!mode.DidRape || !mode.DidCreampie) {
					GalleryLogger.LogDebug($"ManRapeScene: OnEnd: mode {mode.Mode} 'raped' ({mode.DidRape}) or 'creampied' ({mode.DidCreampie}) not set -- event NOT unlocked for {girl}");
					continue;
				}

				OnUnlock(this.man, this.girl, mode.Mode);
			}

			this.man = null;
			this.girl = null;
			this.Modes = new List<ModeInfo>();
			this.CurrentMode = null;
		}
	}
}