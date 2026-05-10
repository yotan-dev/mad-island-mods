using System;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	public class WorldObjectInstantiator : PrefabInstantiator
	{
		public override GameObject CreatePrefab(Vector3 position) {
			throw new Exception("WorldObjectInstantiator should not be used to Create prefabs.");
		}
	}
}
