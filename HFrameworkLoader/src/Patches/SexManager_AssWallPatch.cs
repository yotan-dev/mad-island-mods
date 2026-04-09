using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_AssWallPatch
	{
		[HarmonyPatch(typeof(SexManager), "AssWall")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_AssWall(InventorySlot tmpWall, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_AssWall(tmpWall, ref __result);
		}
	}
}
