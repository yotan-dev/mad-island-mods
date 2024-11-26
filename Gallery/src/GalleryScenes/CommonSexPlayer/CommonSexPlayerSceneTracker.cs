using System;
using System.Collections.Generic;
using YotanModCore;
using Gallery.Patches;
using Gallery.SaveFile.Containers;
using Gallery.Patches.CommonSexPlayer;

namespace Gallery.GalleryScenes.CommonSexPlayer
{
	public class CommonSexPlayerSceneTracker
	{
		public delegate void UnlockInfo(GalleryChara player, GalleryChara npc, int sexType, int specialFlag);

		public event UnlockInfo OnUnlock;

		private GalleryChara Player = null;
		
		private GalleryChara Npc = null;

		private bool DidNormal = false;

		private bool DidCreampie = false;

		private int SexType = -1;

		private int SpecialFlag = -1;

		private bool Busted = false;

		public CommonSexPlayerSceneTracker()
		{
			CommonSexPlayerBasePatch.OnStart += this.OnStart;
			CommonSexPlayerBasePatch.OnBusted += this.OnBusted;
			CommonSexPlayerBasePatch.OnEnd += this.OnEnd;
			SexCountPatch.OnNormal += this.OnNormalCount;
			SexCountPatch.OnCreampie += this.OnCreampieCount;
		}

		private void OnStart(CommonSexPlayerBasePatch.CommonSexPlayerInfo info)
		{
			GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnStart");
			if (this.Player != null || this.Npc != null) {
				GalleryLogger.LogError($"CommonSexPlayerSceneTracker#OnStart: Already active. player: {this.Player} / npc: {this.Npc} -- Dropping previous scene");
			}

			this.Player = new GalleryChara(info.Player);
			this.Npc = new GalleryChara(info.Npc);
			this.DidNormal = false;
			this.DidCreampie = false;
			this.SexType = info.SexType;
			this.SpecialFlag = -1;
			this.Busted = false;
		}

		public void OnNormalCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != this.Player?.OriginalChara || value.to != this.Npc?.OriginalChara) {
				// LoveLogger.LogDebug($"CommonSexPlayerSceneTracker#OnToiletCount: From ({value.from?.charaName}) != player / To != npc ({value.to?.charaName})");
				return;
			}

			this.DidNormal = true;
		}

		public void OnCreampieCount(object sender, SexCountPatch.SexCountChangeInfo value)
		{
			if (value.from != this.Player?.OriginalChara || value.to != this.Npc?.OriginalChara) {
				// LoveLogger.LogDebug($"CommonSexPlayerSceneTracker#OnCreampieCount: From ({value.from?.charaName}) != player / To != npc ({value.to?.charaName})");
				return;
			}

			this.DidCreampie = true;
		}

		private void OnBusted(int specialFlag)
		{
			this.SpecialFlag = specialFlag;
			this.Busted = true;
		}

		private void OnEnd(CommonSexPlayerBasePatch.CommonSexPlayerInfo info)
		{
			if (this.Player?.OriginalChara != info.Player || this.Npc?.OriginalChara != info.Npc) {
				GalleryLogger.LogError($"CommonSexPlayerSceneTracker#OnEnd: CommonSexPlayer ended with different characters. Ignoring.");
				return;
			}

			if (this.SpecialFlag == -1) {
				GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnEnd: 'SpecialFlag' not set -- event NOT unlocked for {this.Npc}");
				return;
			}

			if (this.SpecialFlag > 0 && this.Busted) {
				this.OnUnlock?.Invoke(this.Player, this.Npc, this.SexType, this.SpecialFlag);
			} else if (this.SpecialFlag == 0 && this.DidCreampie && this.DidNormal) {
				this.OnUnlock?.Invoke(this.Player, this.Npc, this.SexType, this.SpecialFlag);
			} else {
				GalleryLogger.LogDebug($"CommonSexPlayerSceneTracker#OnEnd: Conditions not matched (SpecialFlag: {this.SpecialFlag}) / DidNormal: {this.DidNormal} / DidCreampie: {this.DidCreampie} / Busted: {this.Busted}) -- event NOT unlocked for {this.Npc}");
			}
		}
	}
}