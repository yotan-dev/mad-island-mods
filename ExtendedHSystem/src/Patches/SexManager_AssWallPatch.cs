using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
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
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();
			return false;
		}
	}
}
