using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_SlavePatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Slave))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(InventorySlot tmpSlave, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_Slave(tmpSlave, ref __result);
		}

		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Slave))]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_Slave(IEnumerator result)
		{
			return Plugin.Bridge.SexManager_Post_Slave(result);
		}
	}
}
