using HarmonyLib;
using YotanModCore;

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
		private static void Post_InventoryManagerPatches_SubInventoryLoad(InventorySlot chest, ItemData tmpData, InventoryManager __instance)
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

			// In co-op, world benches go through the host-authoritative shared
			// storage/craft flow; opening the local craft panel would conflict.
			if (Coop.IsSupported && Coop.InNetworkScene(__instance))
				return;

			CraftDB.OpenCustomCraftStationForObject(chest, tmpData);
		}
	}
}
