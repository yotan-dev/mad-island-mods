using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_SlavePatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), "Slave")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(InventorySlot tmpSlave, ref IEnumerator __result)
		{
			var scene = new Slave(CommonUtils.GetActivePlayer(), tmpSlave);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}