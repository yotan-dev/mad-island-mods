using HarmonyLib;

namespace YotanModCore.Items.Patches
{
	public static class InventoryManagerPatches
	{
		/// <summary>
		/// Patches InventoryManager.SubInventoryLoad to also load custom Craft benches
		/// </summary>
		/// <param name="chest"></param>
		/// <param name="tmpData"></param>
		/// <returns></returns>
		[HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.SubInventoryLoad))]
		[HarmonyPostfix]
		private static void Post_InventoryManagerPatches_SubInventoryLoad(InventorySlot chest, ItemData tmpData)
		{
			/**
			 * SubInventoryLoad has a switch/case for Bench where it checks the item name in tmpData.name
			 * and calls InventoryManager.CraftOpen for the right CraftData.
			 *
			 * This patch checks for custom benches and call their respective CraftOpen.
			 *
			 * Note that official craft stations are not affected.
			 */
			if (chest == null || tmpData == null || chest.type != InventorySlot.Type.Bench)
				return;

			string benchKey = tmpData.name;
			if (benchKey == null)
				return;

			if (CraftDB.IsDefaultBench(benchKey))
				return; // This should have been handled by the game

			Managers.mn.inventory.CraftOpen(CraftDB.Instance.GetCraftIdForBench(benchKey));
		}
	}
}
