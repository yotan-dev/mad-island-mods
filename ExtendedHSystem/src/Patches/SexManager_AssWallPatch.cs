using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_AssWallPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), "AssWall")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_AssWall(int state, InventorySlot tmpWall, ref IEnumerator __result)
		{
			var pCommon = CommonUtils.GetActivePlayer();
			var girlCommon = Managers.mn.inventory.itemSlot[50].common;
			var scene = new AssWall(pCommon, girlCommon, tmpWall);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}