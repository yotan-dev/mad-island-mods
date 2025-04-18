using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
{
	public class SexManager_ToiletPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Toilet))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Toilet(InventorySlot tmpToile, ref IEnumerator __result)
		{
			var scene = new Toilet(CommonUtils.GetActivePlayer(), Managers.mn.inventory.itemSlot[50].common, tmpToile);
			__result = scene.Run();
			return false;
		}
	}
}
