using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_DarumaPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), "DarumaSex")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_DarumaSex(int state, InventorySlot tmpDaruma, ref IEnumerator __result)
		{
			var pCommon = CommonUtils.GetActivePlayer();
			var girlCommon = Managers.mn.inventory.itemSlot[50].common;
			
			var scene = new Daruma(pCommon, girlCommon, tmpDaruma);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();

			return false;
		}
	}
}