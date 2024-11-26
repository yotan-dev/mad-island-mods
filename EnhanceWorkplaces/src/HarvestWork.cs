using UnityEngine;
using YotanModCore;

namespace EnhanceWorkplaces
{
	internal class HarvestWork
	{
		public static void GiveRewards(CommonStates common, WorkPlace workPlace, InventorySlot tmpInventory, int posID, NPCManager __instance)
		{
			if (workPlace == null)
				return;

			var itemData = __instance.WorkReward(NPCMove.WorkType.Harvest, workPlace.groundID, 1);
			Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, 1);
		}

		public static bool TryCollectingFishTraps(CommonStates common, WorkPlace workPlace, InventorySlot tmpInventory, int posID, NPCManager __instance)
		{
			Vector3 position = common.gameObject.transform.position;
			for (int i = 0; i < Managers.mn.buildMN.breedList.Count; i++)
			{
				BreedInfo breedInfo = Managers.mn.buildMN.breedList[i];
				var inventory = breedInfo.GetComponent<InventorySlot>();
				float distance = Vector3.Distance(position, breedInfo.transform.position);
				if (breedInfo.trapType == BreedInfo.TrapType.FishSmall && distance <= 10f && inventory?.slots[0].stack > 0) {
					var itemData = Managers.mn.itemMN.FindItem(inventory.slots[0].itemKey);
					Managers.mn.inventory.ConsumeSlotItem(inventory.slots[0], 1);
					Managers.mn.itemMN.ItemToChest(itemData, tmpInventory, inventory.slots[0].stack);
					return true;
				}
			}

			return false;
		}
	}
}