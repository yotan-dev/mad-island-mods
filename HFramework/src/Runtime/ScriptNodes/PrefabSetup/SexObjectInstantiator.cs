using System;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	public class SexObjectInstantiator : PrefabInstantiator
	{
		public override GameObject CreatePrefab(Vector3 position) {
			throw new Exception("SexObjectInstantiator should not be used to Create prefabs.");
		}
	}
}
