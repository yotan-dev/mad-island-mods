using Gallery.SaveFile.Containers;
using ExtendedHSystem;
using System.Collections;
using ExtendedHSystem.Scenes;

namespace Gallery.GalleryScenes.Toilet
{
	public class ToiletSceneEventHandler : SceneEventHandler
	{
		public GalleryChara User;

		public GalleryChara Target;

		public bool DidToilet;

		public bool DidCreampie;

		public ToiletSceneEventHandler(CommonStates user, CommonStates target) : base("yogallery_toilet_handler")
		{
			this.User = new GalleryChara(user);
			this.Target = new GalleryChara(target);
		}

		public override IEnumerable OnToilet(CommonStates from, CommonStates to)
		{
			this.DidToilet = true;
			yield return null;
		}

		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			this.DidCreampie = true;
			yield return null;
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			if (!this.DidToilet || !this.DidCreampie)
			{
				GalleryLogger.LogDebug($"ToiletSceneTracker#OnEnd: 'DidCreampie'/'DidToilet' not set -- event NOT unlocked for {this.User} x {this.Target}");
				yield break;
			}

			ToiletSceneManager.Instance.Unlock(this.User, this.Target);
		}
	}
}