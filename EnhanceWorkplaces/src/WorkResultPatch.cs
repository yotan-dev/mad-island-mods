using HarmonyLib;
using YotanModCore;

namespace EnhanceWorkplaces
{
	public class WorkResultPatch
	{
		[HarmonyPatch(typeof(NPCManager), "WorkResult")]
		[HarmonyPrefix]
		private static void Pre_NPCManager_WorkResult(CommonStates common, WorkPlace workPlace, InventorySlot tmpInventory, int posID, NPCManager __instance, ref bool __runOriginal)
		{
			if (common.nMove.workType != NPCMove.WorkType.Wood && common.nMove.workType != NPCMove.WorkType.Stone && common.nMove.workType != NPCMove.WorkType.Harvest)
				return;

			__runOriginal = false;

			WorkplacesCommon.OnWorkComplete(common);
			switch (common.nMove.workType)
			{
				case NPCMove.WorkType.Harvest:
					if (GameInfo.GameVersion <= GameInfo.ToVersion("0.1.8")) {
						// v0.2.0 officially supports that, so we don't need to patch it.
						if (common.bag.slots[0]?.itemKey == "wp_fishingrod_01") {
							if (!HarvestWork.TryCollectingFishTraps(common, workPlace, tmpInventory, posID, __instance)) {
								HarvestWork.GiveRewards(common, workPlace, tmpInventory, posID, __instance);
							}
						}
					}
					break;

				case NPCMove.WorkType.Wood:
					WoodWork.GiveRewards(common, workPlace, tmpInventory, posID, __instance);
					break;

				case NPCMove.WorkType.Stone:
					StoneWork.GiveRewards(common, workPlace, tmpInventory, posID, __instance);
					break;
			}
		}
	}
}