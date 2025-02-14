using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes.PlayerRaped
{
	public class PlayerRapedTracker : BaseTracker
	{
		private readonly GalleryChara Player;
		private readonly GalleryChara Rapist;

		public PlayerRapedTracker(
			CommonStates player,
			CommonStates rapist
		) : base()
		{
			this.Player = new GalleryChara(player);
			this.Rapist = new GalleryChara(rapist);
		}

		public override void End()
		{
			if (this.Raped)
			{
				new PlayerRapedController().Unlock(this.PerformerId, [this.Player, this.Rapist]);
			}
			else
			{
				GalleryLogger.LogDebug($"PlayerRapedTracker: AfterRape: 'DidRape' not set -- event NOT unlocked for {Rapist}");
			}
		}

		public void LoadPerformerId()
		{
			this.PerformerId = GalleryScenesManager.Instance.FindPerformer(typeof(PlayerRapedController), [this.Rapist, this.Player]);
		}
	}
}
