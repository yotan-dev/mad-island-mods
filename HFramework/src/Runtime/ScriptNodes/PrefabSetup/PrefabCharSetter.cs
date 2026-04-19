using System;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class PrefabCharSetter : AppearanceSetter
	{
		public override void SetAppearance(GameObject prefab, CommonContext ctx) {
			prefab.GetComponent<IAppearanceSetter>().SetAppearance(prefab, ctx);
		}
	}
}
