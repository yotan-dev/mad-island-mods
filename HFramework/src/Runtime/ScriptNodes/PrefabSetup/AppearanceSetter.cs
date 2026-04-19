using System;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class AppearanceSetter
	{
		public virtual void SetAppearance(GameObject prefab, CommonContext ctx) {
			throw new NotImplementedException();
		}
	}
}
