using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class SexListPrefab : PrefabInstantiator
	{
		public int listIndex;
		public int objIndex;

		public override GameObject CreatePrefab(Vector3 position) {
			var prefab = Managers.sexMN.sexList[listIndex].sexObj[objIndex];
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}
	}
}
