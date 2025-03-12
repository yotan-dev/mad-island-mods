using HarmonyLib;
using YotanModCore;

namespace EnhancedIsland.StackNearby
{
	public class BuildInfoPatch
	{
		[HarmonyPatch(typeof(BuildManager), "Awake")]
		[HarmonyPrefix]
		private static void Pre_BuildManager_Awake()
		{
			StackNearbyController.Storages.Clear();
		}


		[HarmonyPatch(typeof(BuildManager), "AddBuild")]
		[HarmonyPrefix]
		private static void Pre_BuildManager_AddBuild(ItemInfo tmpInfo)
		{
			if (tmpInfo == null) {
				return;
			}

			var itemData = Managers.mn.itemMN.FindItem(tmpInfo.itemKey);
			if (itemData.subType == ItemData.SubType.Chest) {
				StackNearbyController.Storages.Add(tmpInfo);
			}
		}

		[HarmonyPatch(typeof(BuildManager), "RemoveBuild")]
		[HarmonyPrefix]
		private static void Pre_BuildManager_RemoveBuild(ItemInfo tmpInfo)
		{
			if (tmpInfo == null) {
				return;
			}

			var itemData = Managers.mn.itemMN.FindItem(tmpInfo.itemKey);
			if (itemData.subType == ItemData.SubType.Chest) {
				StackNearbyController.Storages.Remove(tmpInfo);
			}
		}
	}
}
