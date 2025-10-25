using HarmonyLib;

namespace YotanModCore.Items.Patches
{
	public static class ItemManagerPatches
	{
		/// <summary>
		/// Patches ItemManager.FindItem to also lookup into custom ItemDB
		/// </summary>
		/// <param name="__result"></param>
		/// <param name="itemKey"></param>
		/// <returns></returns>
		[HarmonyPatch(typeof(ItemManager), nameof(ItemManager.FindItem))]
		[HarmonyPostfix]
		private static ItemData Post_ItemManager_FindItem(ItemData __result, string itemKey)
		{
			// The game already contains this item
			if (__result != null)
				return __result;

			return ItemDB.Instance.GetItem(itemKey);
		}
	}
}
