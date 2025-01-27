using System.Collections.Generic;
using HFramework.Scenes;
using Gallery.SaveFile.Containers;
using Gallery.GalleryScenes.CommonSexNPC;

namespace Gallery.GalleryScenes.ManSleepRape
{
	public class ManSleepRapeTracker : BaseTracker
	{
		private class ModeInfo
		{
			public ManRapeSleepState Mode;

			public bool DidRape;

			public bool DidCreampie;
		}

		private readonly GalleryChara Man;
		private readonly GalleryChara Girl;

		private readonly List<ModeInfo> Modes = [];

		private ModeInfo CurrentMode = null;

		public override bool DidCreampie { get => this.CurrentMode.DidCreampie; set => this.CurrentMode.DidCreampie = value; }

		public override bool Raped { get => this.CurrentMode.DidRape; set => this.CurrentMode.DidRape = value; }

		public ManSleepRapeTracker(CommonStates man, CommonStates girl) : base()
		{
			this.Man = new GalleryChara(man);
			this.Girl = new GalleryChara(girl);
		}

		public void OnSleepRapeTypeChange(ManRapeSleepState type)
		{
			var newMode = this.Modes.Find((mode) => mode.Mode == type);
			if (newMode == null)
			{
				newMode = new ModeInfo();
				newMode.Mode = type;
				this.Modes.Add(newMode);
			}

			CurrentMode = newMode;
		}

		public override void End()
		{
			bool shouldUnlock = false;
			foreach (var mode in this.Modes)
			{

				if (mode.DidRape && mode.DidCreampie)
				{
					shouldUnlock = true;
					break;
				}
			}

			if (!shouldUnlock)
			{
				GalleryLogger.LogDebug($"ManSleepRapeScene: OnEnd: 'raped' and 'creampied' not set for the same mode -- event NOT unlocked for {Girl}");
				return;
			}

			new ManSleepRapeController().Unlock([this.Man, this.Girl]);
		}
	}
}
