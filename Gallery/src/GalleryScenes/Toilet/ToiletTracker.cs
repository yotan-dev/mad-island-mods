using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Toilet
{
	public class ToiletTracker : BaseTracker
	{
		public GalleryChara User;

		public GalleryChara Target;

		public ToiletTracker(CommonStates user, CommonStates target) : base()
		{
			this.User = new GalleryChara(user);
			this.Target = new GalleryChara(target);
		}

		public override void End()
		{
			if (!this.DidToilet || !this.DidCreampie)
			{
				GalleryLogger.LogDebug($"ToiletSceneTracker#OnEnd: 'DidCreampie'/'DidToilet' not set -- event NOT unlocked for {this.User} x {this.Target}");
				return;
			}

			ToiletSceneManager.Instance.Unlock(this.User, this.Target);
		}
	}
}
