using Gallery.SaveFile.Containers;
using YotanModCore;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.CommonSexPlayer
{
	public class CommonSexPlayerTracker : BaseTracker
	{
		public readonly GalleryChara Player = null;

		public readonly GalleryChara Npc = null;

		public readonly int SexType = -1;

		public int SpecialFlag = -1;


		public CommonSexPlayerTracker(
			CommonStates player,
			CommonStates npc,
			int sexType
		) : base()
		{
			this.Player = new GalleryChara(player);
			this.Npc = new GalleryChara(npc);
			this.SexType = sexType;
		}

		public override void End()
		{
			if (this.SpecialFlag == -1)
			{
				GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnEnd: 'SpecialFlag' not set -- event NOT unlocked for {this.Npc}");
				return;
			}

			var ctrler = new CommonSexPlayerController() {
				SexType = this.SexType,
				SpecialFlag = this.SpecialFlag,
			};

			if (this.SpecialFlag > 0 && this.Busted)
				ctrler.Unlock(this.PerformerId, [this.Player, this.Npc]);
			else if (this.SpecialFlag == 0 && this.DidCreampie && this.DidNormal)
				ctrler.Unlock(this.PerformerId, [this.Player, this.Npc]);
			else
				GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnEnd: Conditions not matched (SpecialFlag: {this.SpecialFlag}) / DidNormal: {this.DidNormal} / DidCreampie: {this.DidCreampie} / Busted: {this.Busted}) -- event NOT unlocked for {this.Npc}");
		}

		public void LoadPerformerId()
		{
			if (this.Player.Id == NpcID.Man)
				this.PerformerId = GalleryScenesManager.Instance.FindPerformer(typeof(CommonSexPlayerController), [this.Player, this.Npc]);
			else
				this.PerformerId = GalleryScenesManager.Instance.FindPerformer(typeof(CommonSexPlayerController), [this.Npc, this.Player]);
		}
	}
}
