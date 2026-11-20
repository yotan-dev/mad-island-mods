using System;
using HFramework.FreeMode;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class CustomPrefab : PrefabInstantiator
	{
		public GameObject Prefab;

		public override GameObject CreatePrefab(Vector3 position) {
			return FreeModeInterop.SpawnVisual(Prefab, position, Quaternion.identity);
		}
	}
}
