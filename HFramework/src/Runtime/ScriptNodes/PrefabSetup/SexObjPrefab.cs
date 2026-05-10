using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class SexObjPrefab : PrefabInstantiator
	{
		[Tooltip("Index of sexMn.sexObj to use")]
		public int ObjIndex;

		public override GameObject CreatePrefab(Vector3 position) {
			var prefab = Managers.sexMN.sexObj[ObjIndex];
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}
	}
}
