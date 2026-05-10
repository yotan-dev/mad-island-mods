using System;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class BossNativeCharSetter : AppearanceSetter
	{
		[SerializeField, ActorIndex]
		private int BossNativeIndex = 0;

		[SerializeField, ActorIndex]
		private int FemaleIndex = 1;

		public BossNativeCharSetter() { }

		public BossNativeCharSetter(int bossNativeIndex, int femaleIndex) {
			this.BossNativeIndex = bossNativeIndex;
			this.FemaleIndex = femaleIndex;
		}

		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[FemaleIndex].Common, null);

			// Unnofficial fix - Only hide girls if you have rescued them
			if (Managers.mn.story.QuestProgress("Sub_Kana") != 0) {
				ctx.Actors[BossNativeIndex].Common.anim.state.SetAnimation(10, "Hide_G1", true);
				ctx.Actors[BossNativeIndex].Common.anim.state.SetAnimation(11, "Hide_G2", true);
			}
		}
	}
}
