using UnityEngine;
using YotanModCore;

namespace HFramework.Tree.PrefabCreators
{
	public class SexListPrefab : BasePrefabCreator
	{
		[SerializeField] private int listIndex;
		[SerializeField] private int objIndex;

		public SexListPrefab(int listIndex, int objIndex)
		{
			this.listIndex = listIndex;
			this.objIndex = objIndex;
		}

		public override GameObject CreatePrefab(Vector3 position)
		{
			var prefab = Managers.sexMN.sexList[listIndex].sexObj[objIndex];
			return GameObject.Instantiate(prefab, position, Quaternion.identity);
		}

		public override void SetAppearance(GameObject prefab, CommonContext ctx)
		{
			Managers.mn.randChar.SetCharacter(prefab, ctx.NpcB, ctx.NpcA);
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
	}
}
