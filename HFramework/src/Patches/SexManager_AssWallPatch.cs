using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
{
	public class SexManager_AssWallPatch
	{
		[HarmonyPatch(typeof(SexManager), "AssWall")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_AssWall(int state, InventorySlot tmpWall, ref IEnumerator __result)
		{
			var pCommon = CommonUtils.GetActivePlayer();
			var girlCommon = Managers.mn.inventory.itemSlot[50].common;
			var scene = new AssWall(pCommon, girlCommon, tmpWall);
			__result = scene.Run();
			return false;
		}
	}
}
