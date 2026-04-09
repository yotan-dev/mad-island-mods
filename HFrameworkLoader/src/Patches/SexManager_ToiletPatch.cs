using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_ToiletPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Toilet))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Toilet(InventorySlot tmpToile, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_Toilet(tmpToile, ref __result);
		}
	}
}
