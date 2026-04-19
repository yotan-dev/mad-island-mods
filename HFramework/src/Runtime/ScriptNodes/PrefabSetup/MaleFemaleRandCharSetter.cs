using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class MaleFemaleRandCharSetter : AppearanceSetter
	{
		[SerializeField] private int MaleIndex = 0;
		[SerializeField] private int FemaleIndex = 1;

		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[FemaleIndex].Common, ctx.Actors[MaleIndex].Common);
		}
	}
}
