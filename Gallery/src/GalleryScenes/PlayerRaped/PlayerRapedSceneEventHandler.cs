using System;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;
using ExtendedHSystem;
using System.Collections;

namespace Gallery.GalleryScenes.PlayerRaped
{
	public class PlayerRapedSceneEventHandler : SceneEventHandler
	{
		public delegate void OnSceneInfo(GalleryChara player, GalleryChara rapist);

		private readonly GalleryChara Player;
		private readonly GalleryChara Rapist;

		private bool DidRape;

		public PlayerRapedSceneEventHandler(
			CommonStates player,
			CommonStates rapist
		) : base("yogallery_playerraped_handler") {
			this.Player = new GalleryChara(player);
			this.Rapist = new GalleryChara(rapist);
		}

		public override IEnumerable PlayerRaped(CommonStates player, CommonStates rapist, bool silent)
		{
			this.DidRape = true;
			return base.PlayerRaped(player, rapist, silent);
		}

		public override IEnumerable AfterRape(CommonStates victim, CommonStates rapist)
		{
			if (this.DidRape) {
				PlayerRapedSceneManager.Instance.Unlock(this.Player, this.Rapist);
			} else {
				GalleryLogger.LogDebug($"PlayerRapedSceneEventHandler: AfterRape: 'DidRape' not set -- event NOT unlocked for {Rapist}");
			}
			return base.AfterRape(victim, rapist);
		}
	}
}