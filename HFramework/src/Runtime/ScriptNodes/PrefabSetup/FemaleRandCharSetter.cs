using System;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class FemaleRandCharSetter : AppearanceSetter
	{
		[SerializeField, ActorIndex]
		private int FemaleIndex = 1;

		public FemaleRandCharSetter() { }

		public FemaleRandCharSetter(int femaleIndex) {
			this.FemaleIndex = femaleIndex;
		}

		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[FemaleIndex].Common, null);
		}
	}
}
