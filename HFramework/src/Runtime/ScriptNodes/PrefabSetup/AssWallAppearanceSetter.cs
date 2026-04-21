using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class AssWallAppearanceSetter : AppearanceSetter
	{
		public string SkinName = "Man";

		[ActorIndex]
		public int MaleActor = 0;

		[ActorIndex]
		public int GirlActor = 1;

		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			ctx.TmpSexAnim.skeleton.SetSkin(this.SkinName);
			ctx.TmpSexAnim.skeleton.SetSlotsToSetupPose();
			Managers.mn.randChar.SetCharacter(prefab, null, ctx.Actors[this.MaleActor].Common);
			Managers.mn.randChar.SetAssWall(ctx.Actors[this.GirlActor].Common, ctx.TmpSexAnim.gameObject);
		}
	}
}
