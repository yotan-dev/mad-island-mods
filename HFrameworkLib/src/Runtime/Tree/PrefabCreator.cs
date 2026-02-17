using System;
using Spine.Unity;
using UnityEngine;
using YotanModCore;

namespace HFramework.Tree
{
	[Serializable]
	public class PrefabConfig
	{
		public enum PrefabType
		{
			SexList,
			Prefab,
		}

		public enum AppearenceMode
		{
			MaleFemale,
			// GirlGirl,
			// Custom,
		}

		[Header("General config (Always required)")]
		[SerializeField] private PrefabType prefabType = default;
		[SerializeField] private AppearenceMode appearanceMode = default;

		[Space(10)]

		[Header("MaleFemale config (if AppearenceMode = MaleFemale)")]
		[Tooltip("Male index in SexScript > Info > Npcs")]
		[SerializeField] private int maleIndex = 0;

		[Tooltip("Female index in SexScript > Info > Npcs")]
		[SerializeField] private int femaleIndex = 1;

		[Space(10)]

		[Header("SexList config (if PrefabType = SexList)")]
		[SerializeField] private int listIndex = 0;
		[SerializeField] private int objIndex = 0;

		[Space(10)]

		[Header("Prefab config (if PrefabType = Prefab)")]
		[SerializeField] private GameObject prefab = default;

		public GameObject CreatePrefab(Vector3 position)
		{
			if (prefabType == PrefabType.SexList)
			{
				var prefab = Managers.sexMN.sexList[listIndex].sexObj[objIndex];
				return GameObject.Instantiate(prefab, position, Quaternion.identity);
			}
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}

		public void SetAppearance(GameObject prefab, CommonContext ctx)
		{
			if (appearanceMode == AppearenceMode.MaleFemale)
			{
				Managers.mn.randChar.SetCharacter(prefab, ctx.Actors[femaleIndex].Common, ctx.Actors[maleIndex].Common);
			}
			else
			{
				PLogger.LogError("Unknown appearance mode: " + appearanceMode);
			}
			// switch (girl.npcID)
			// {
			// 	case 9:
			// 	case 15:
			// 	case 16 /*0x10*/:
			// 	case 73:
			// 		switch (man.npcID)
			// 		{
			// 			case 9:
			// 			case 15:
			// 			case 16 /*0x10*/:
			// 			case 73:
			// 				sexManager.mn.randChar.SetCharacter(tmpSex, girl, (CommonStates)null);
			// 				CommonStates component = tmpSex.GetComponent<CommonStates>();
			// 				if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			// 				{
			// 					sexManager.mn.randChar.CopyParams(man, component);
			// 					sexManager.mn.randChar.LoadGenGirl(tmpSex, loadType: RandomCharacter.LoadType.G);
			// 					break;
			// 				}
			// 				break;
			// 			default:
			// 				sexManager.mn.randChar.SetCharacter(tmpSex, girl, man);
			// 				break;
			// 		}
			// 		break;
			// 	default:
			// 		sexManager.mn.randChar.SetCharacter(tmpSex, girl, man);
			// 		break;
			// }
		}

		public virtual SkeletonAnimation GetSkeletonAnimation(GameObject prefab)
		{
			return prefab.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
		}
	}
}
