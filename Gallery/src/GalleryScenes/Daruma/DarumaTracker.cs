using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.Daruma
{
	public class DarumaTracker : BaseTracker
	{
		private readonly GalleryChara Player = null;

		private readonly GalleryChara Girl = null;


		public DarumaTracker(
			CommonStates player,
			CommonStates girl
		) : base()
		{
			this.Player = new GalleryChara(player);
			this.Girl = new GalleryChara(girl);
		}

		public override void End()
		{
			if (this.Raped && this.Busted)
			{
				new DarumaController().Unlock(this.PerformerId, [this.Player, this.Girl]);
			}
			else
			{
				var desc = $"{this.Player} x {this.Girl}";
				GalleryLogger.LogDebug($"DarumaSceneTracker#OnEnd: 'Raped' ({this.Raped}) or 'Busted' ({this.Busted}) not set -- event NOT unlocked for {desc}");
			}
		}

		public void LoadPerformerId()
		{
			this.PerformerId = GalleryScenesManager.Instance.FindPerformer(typeof(DarumaController), [this.Player, this.Girl]);
		}
	}
}
