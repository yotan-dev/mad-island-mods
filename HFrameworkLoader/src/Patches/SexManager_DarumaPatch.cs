using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_DarumaPatch
	{
		[HarmonyPatch(typeof(SexManager), "DarumaSex")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_DarumaSex(int state, InventorySlot tmpDaruma, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_DarumaSex(state, tmpDaruma, ref __result);
		}
	}
}
