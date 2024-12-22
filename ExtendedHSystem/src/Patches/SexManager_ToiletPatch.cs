using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_ToiletPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Toilet))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Toilet(InventorySlot tmpToile, ref IEnumerator __result)
		{
			var scene = new Toilet(CommonUtils.GetActivePlayer(), Managers.mn.inventory.itemSlot[50].common, tmpToile);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}
