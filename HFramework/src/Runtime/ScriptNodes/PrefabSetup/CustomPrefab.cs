using System;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class CustomPrefab : PrefabInstantiator
	{
		public GameObject Prefab;

		public override GameObject CreatePrefab(Vector3 position) {
			return GameObject.Instantiate(Prefab, position, Quaternion.identity);
		}
	}
}
