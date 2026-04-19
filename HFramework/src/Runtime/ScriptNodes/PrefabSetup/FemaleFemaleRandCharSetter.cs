using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class FemaleFemaleRandCharSetter : AppearanceSetter
	{
		[SerializeField] private int FemaleIndex = 0;

		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[FemaleIndex].Common, ctx.Actors[FemaleIndex].Common);

			// @TODO: Understand when this is used and how to make it into char setter
			// 				CommonStates component = tmpSex.GetComponent<CommonStates>();
			// 				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			// 				{
			// 					sexManager.mn.randChar.CopyParams(man, component);
			// 					sexManager.mn.randChar.LoadGenGirl(tmpSex, loadType: RandomCharacter.LoadType.G);
			// 					break;
			// 				}
		}
	}
}
