using Gallery.SaveFile.Containers;
using ExtendedHSystem;
using System.Collections;
using ExtendedHSystem.Scenes;

namespace Gallery.GalleryScenes.CommonSexPlayer
{
	public class CommonSexPlayerSceneEventHandler : SceneEventHandler
	{
		public readonly GalleryChara Player = null;

		public readonly GalleryChara Npc = null;

		public readonly int SexType = -1;

		private bool DidNormal = false;

		private bool DidCreampie = false;

		private bool Busted = false;

		private int SpecialFlag = -1;


		public CommonSexPlayerSceneEventHandler(
			CommonStates player,
			CommonStates npc,
			int sexType
		) : base("yogallery_commonsexplayer_handler")
		{
			this.Player = new GalleryChara(player);
			this.Npc = new GalleryChara(npc);
			this.SexType = sexType;
		}

		public override IEnumerable OnNormalSex(CommonStates a, CommonStates b)
		{
			this.DidNormal = true;
			return base.OnNormalSex(a, b);
		}

		public override IEnumerable OnBusted(CommonStates from, CommonStates to, int specialFlag)
		{
			this.Busted = true;
			this.SpecialFlag = specialFlag;
			return base.OnBusted(from, to, specialFlag);
		}

		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			this.DidCreampie = true;
			return base.OnCreampie(from, to);
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			if (this.SpecialFlag == -1)
			{
				GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnEnd: 'SpecialFlag' not set -- event NOT unlocked for {this.Npc}");
				return base.AfterSex(scene, from, to);
			}

			if (this.SpecialFlag > 0 && this.Busted)
				CommonSexPlayerSceneManager.Instance.Unlock(this.Player, this.Npc, this.SexType, this.SpecialFlag);
			else if (this.SpecialFlag == 0 && this.DidCreampie && this.DidNormal)
				CommonSexPlayerSceneManager.Instance.Unlock(this.Player, this.Npc, this.SexType, this.SpecialFlag);
			else
				GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnEnd: Conditions not matched (SpecialFlag: {this.SpecialFlag}) / DidNormal: {this.DidNormal} / DidCreampie: {this.DidCreampie} / Busted: {this.Busted}) -- event NOT unlocked for {this.Npc}");

			return base.AfterSex(scene, from, to);
		}
	}
}