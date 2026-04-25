using System;
using Spine.Unity;
using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[Serializable]
	[Experimental]
	public class PrefabInstantiator
	{
		public string PathToSkeletonAnimation = "Scale/Anim";

		public virtual GameObject CreatePrefab(Vector3 position) {
			throw new NotImplementedException();
		}

		public virtual SkeletonAnimation GetSkeletonAnimation(GameObject prefab) {
			return prefab.transform.Find(this.PathToSkeletonAnimation).gameObject.GetComponent<SkeletonAnimation>();
		}
	}
}
