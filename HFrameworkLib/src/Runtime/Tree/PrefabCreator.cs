using System;
using Spine.Unity;
using UnityEngine;
using YotanModCore;

namespace HFramework.Tree
{
	[Serializable]
	public class SexListPrefab : PrefabInstantiator {
		public int listIndex;
		public int objIndex;

		public override GameObject CreatePrefab(Vector3 position)
		{
			var prefab = Managers.sexMN.sexList[listIndex].sexObj[objIndex];
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}
	}

	[Serializable]
	public class CustomPrefab : PrefabInstantiator {
		public GameObject prefab;

		public override GameObject CreatePrefab(Vector3 position)
		{
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}
	}

	[Serializable]
	public class PrefabInstantiator {
		public string PathToSkeletonAnimation = "Scale/Anim";

		public virtual GameObject CreatePrefab(Vector3 position)
		{
			throw new NotImplementedException();
		}

		public virtual SkeletonAnimation GetSkeletonAnimation(GameObject prefab)
		{
			return prefab.transform.Find(this.PathToSkeletonAnimation).gameObject.GetComponent<SkeletonAnimation>();
		}
	}

	public interface IAppearanceSetter {
		void SetAppearance(GameObject prefab, CommonContext ctx);
	}

	[Serializable]
	public class PrefabCharSetter : AppearanceSetter {
		public override void SetAppearance(GameObject prefab, CommonContext ctx)
		{
			prefab.GetComponent<IAppearanceSetter>().SetAppearance(prefab, ctx);
		}
	}

	[Serializable]
	public class FemaleFemaleRandCharSetter : AppearanceSetter
	{
		[SerializeField] private int FemaleIndex = 0;

		public override void SetAppearance(GameObject prefab, CommonContext ctx)
		{
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[FemaleIndex].Common, ctx.Actors[FemaleIndex].Common);

// @TODO: Understand when this is used and how to make it into char setter
// 				CommonStates component = tmpSex.GetComponent<CommonStates>();
// 				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
// 				{
// 					sexManager.mn.randChar.CopyParams(man, component);
// 					sexManager.mn.randChar.LoadGenGirl(tmpSex, loadType: RandomCharacter.LoadType.G);
// 					break;
// 				}
		}
	}

	[Serializable]
	public class MaleFemaleRandCharSetter : AppearanceSetter
	{
		[SerializeField] private int MaleIndex = 0;
		[SerializeField] private int FemaleIndex = 1;

		public override void SetAppearance(GameObject prefab, CommonContext ctx)
		{
			Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[FemaleIndex].Common, ctx.Actors[MaleIndex].Common);
		}
	}

	[Serializable]
	public class AppearanceSetter {
		public virtual void SetAppearance(GameObject prefab, CommonContext ctx)
		{
			throw new NotImplementedException();
		}
	}

	[Serializable]
	public class PrefabConfig
	{
		[SerializeReference, Subclass]
		public PrefabInstantiator Instantiator;

		[SerializeReference, Subclass]
		public AppearanceSetter AppearanceSetter;
	}
}
