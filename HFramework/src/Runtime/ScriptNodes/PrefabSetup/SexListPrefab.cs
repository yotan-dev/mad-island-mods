using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class SexListPrefab : PrefabInstantiator
	{
		[Tooltip("Index of the sex list to use")]
		public int ListIndex;

		[Tooltip("Index of the object within the sex list to use")]
		public int ObjIndex;

		public override GameObject CreatePrefab(Vector3 position) {
			var prefab = Managers.sexMN.sexList[ListIndex].sexObj[ObjIndex];
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}
	}
}
