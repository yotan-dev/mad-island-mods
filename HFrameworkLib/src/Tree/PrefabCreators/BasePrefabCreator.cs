using Spine.Unity;
using UnityEngine;

namespace HFramework.Tree.PrefabCreators
{
	public abstract class BasePrefabCreator
	{
		public abstract GameObject CreatePrefab(Vector3 position);

		public abstract void SetAppearance(GameObject prefab, CommonContext ctx);

		public virtual SkeletonAnimation GetSkeletonAnimation(GameObject prefab)
		{
			return prefab.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
		}
	}
}
