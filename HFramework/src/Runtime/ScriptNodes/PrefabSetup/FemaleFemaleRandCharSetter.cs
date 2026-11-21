using System;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class FemaleFemaleRandCharSetter : AppearanceSetter
	{
		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[0].Common, null);

			var tmpSexStates = prefab?.GetComponent<CommonStates>();
			if (tmpSexStates != null) {
				Managers.mn.randChar.CopyParams(ctx.Actors[1].Common, tmpSexStates);
				Managers.mn.randChar.LoadGenGirl(prefab, loadType: RandomCharacter.LoadType.G);
			}
		}
	}
}
