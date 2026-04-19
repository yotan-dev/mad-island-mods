using System;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class CustomPrefab : PrefabInstantiator
	{
		public GameObject prefab;

		public override GameObject CreatePrefab(Vector3 position) {
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}
	}
}
