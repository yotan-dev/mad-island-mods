using System.Collections;
using HFramework;
using HFramework.Scenes;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Daruma
{
	public class DarumaSceneEventHandler : SceneEventHandler
	{
		private readonly GalleryChara Player = null;

		private readonly GalleryChara Girl = null;

		private bool DidRape = false;

		private bool DidBust = false;

		public DarumaSceneEventHandler(
			CommonStates player,
			CommonStates girl
		) : base("yogallery_daruma_handler")
		{
			this.Player = new GalleryChara(player);
			this.Girl = new GalleryChara(girl);
		}

		public override IEnumerable OnRape(CommonStates from, CommonStates to)
		{
			this.DidRape = true;
			return base.OnRape(from, to);
		}

		public override IEnumerable OnBusted(CommonStates from, CommonStates to, int specialFlag)
		{
			this.DidBust = true;
			return base.OnBusted(from, to, specialFlag);
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			if (this.DidRape && this.DidBust)
			{
				DarumaSceneManager.Instance.Unlock(this.Player, this.Girl);
			}
			else
			{
				var desc = $"{this.Player} x {this.Girl}";
				GalleryLogger.LogDebug($"DarumaSceneTracker#OnEnd: 'DidRape' ({this.DidRape}) or 'DidBust' ({this.DidBust}) not set -- event NOT unlocked for {desc}");
			}

			return base.AfterSex(scene, from, to);
		}
	}
}
