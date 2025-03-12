using YotanModCore;

namespace EnhancedIsland.BetterWorkplaces
{
	internal class WoodWork
	{
		public static void GiveRewards(CommonStates common, WorkPlace workPlace, InventorySlot tmpInventory, int posID, NPCManager __instance)
		{
			if (workPlace == null)
				return;

			var itemData = __instance.WorkReward(NPCMove.WorkType.Wood, workPlace.groundID, 1);
			Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, WorkplacesCommon.GetLv1Quantity(common));
			if (UnityEngine.Random.Range(0, 100) <= 10)
			{
				itemData = __instance.WorkReward(NPCMove.WorkType.Wood, workPlace.groundID, 2);
				Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, WorkplacesCommon.GetLv2Quantity(common));
				return;
			}
		}
	}
}
