using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem;
using ExtendedHSystem.Scenes;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.ManSleepRape
{
	public class ManSleepRapeSceneEventHandler : SceneEventHandler
	{
		private class ModeInfo {
			public ManRapeSleepState Mode;

			public bool DidRape;

			public bool DidCreampie;
		}

		private GalleryChara Man;
		private GalleryChara Girl;

		private List<ModeInfo> Modes = new List<ModeInfo>();

		private ModeInfo CurrentMode = null;

		public ManSleepRapeSceneEventHandler(CommonStates man, CommonStates girl) : base("yogallery_mansleeprape_handler") {
			this.Man = new GalleryChara(man);
			this.Girl = new GalleryChara(girl);
		}

		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			if (this.CurrentMode == null)
				yield break;

			this.CurrentMode.DidCreampie = true;
		}

		public override IEnumerable OnRape(CommonStates from, CommonStates to)
		{
			if (this.CurrentMode == null)
				yield break;

			this.CurrentMode.DidRape = true;
		}

		public override IEnumerable OnSleepRapeTypeChange(IScene scene, ManRapeSleepState type)
		{
			var newMode = this.Modes.Find((mode) => mode.Mode == type);
			if (newMode == null) {
				newMode = new ModeInfo();
				newMode.Mode = type;
				this.Modes.Add(newMode);
			}

			CurrentMode = newMode;
			
			yield break;
		}

		public override IEnumerable AfterManRape(CommonStates victim, CommonStates rapist)
		{
			foreach (var mode in this.Modes) {
				if (!mode.DidRape || !mode.DidCreampie) {
					GalleryLogger.LogDebug($"ManRapeScene: OnEnd: mode {mode.Mode} 'raped' ({mode.DidRape}) or 'creampied' ({mode.DidCreampie}) not set -- event NOT unlocked for {Girl}");
					continue;
				}

				ManSleepRapeSceneManager.Instance.Unlock(this.Man, this.Girl, mode.Mode);
			}

			yield break;
		}
	}
}