using Spine.Unity;
using UnityEngine;

namespace HFramework.SexScripts.PrefabCreators
{
	[Experimental]
	public abstract class BasePrefabCreator
	{
		public abstract GameObject CreatePrefab(Vector3 position);

		public abstract void SetAppearance(GameObject prefab, CommonSexNpcScript ctx);

		public virtual SkeletonAnimation GetSkeletonAnimation(GameObject prefab)
		{
			return prefab.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
		}
	}
}
