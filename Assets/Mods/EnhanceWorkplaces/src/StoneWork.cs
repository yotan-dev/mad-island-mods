using YotanModCore;

namespace EnhanceWorkplaces
{
	internal class StoneWork
	{
		public static void GiveRewards(CommonStates common, WorkPlace workPlace, InventorySlot tmpInventory, int posID, NPCManager __instance)
		{
			if (workPlace == null)
				return;

			var itemData = __instance.WorkReward(NPCMove.WorkType.Stone, workPlace.groundID, 1);
			Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, WorkplacesCommon.GetLv1Quantity(common));
			
			if (UnityEngine.Random.Range(0, 100) <= 2)
			{
				itemData = __instance.WorkReward(NPCMove.WorkType.Stone, workPlace.groundID, 3);
				Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, WorkplacesCommon.GetLv3Quantity(common));
				return;
			}

			if (UnityEngine.Random.Range(0, 100) <= 10)
			{
				itemData = __instance.WorkReward(NPCMove.WorkType.Stone, workPlace.groundID, 2);
				Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, WorkplacesCommon.GetLv2Quantity(common));
				return;
			}
		}
	}
}