using HarmonyLib;
using YotanModCore;

namespace EnhancedIsland.BetterWorkplaces
{
	public class WorkResultPatch
	{
		[HarmonyPatch(typeof(NPCManager), "WorkResult")]
		[HarmonyPrefix]
		private static void Pre_NPCManager_WorkResult(CommonStates common, WorkPlace workPlace, InventorySlot tmpInventory, int posID, NPCManager __instance, ref bool __runOriginal)
		{
			if (common.nMove.workType != NPCMove.WorkType.Wood && common.nMove.workType != NPCMove.WorkType.Stone && common.nMove.workType != NPCMove.WorkType.Harvest)
				return;

			switch (common.nMove.workType)
			{
				case NPCMove.WorkType.Harvest:
					// v0.2.0 officially supports that, so we don't need to patch it.
					if (GameInfo.GameVersion <= GameInfo.ToVersion("0.1.8")) {
						if (common.bag.slots[0]?.itemKey == "wp_fishingrod_01") {
							if (!HarvestWork.TryCollectingFishTraps(common, workPlace, tmpInventory, posID, __instance)) {
								__runOriginal = false;
								WorkplacesCommon.OnWorkComplete(common);
								HarvestWork.GiveRewards(common, workPlace, tmpInventory, posID, __instance);
							}
						}
					}
					break;

				case NPCMove.WorkType.Wood:
					__runOriginal = false;
					WorkplacesCommon.OnWorkComplete(common);
					WoodWork.GiveRewards(common, workPlace, tmpInventory, posID, __instance);
					break;

				case NPCMove.WorkType.Stone:
					__runOriginal = false;
					WorkplacesCommon.OnWorkComplete(common);
					StoneWork.GiveRewards(common, workPlace, tmpInventory, posID, __instance);
					break;
			}
		}
	}
}
