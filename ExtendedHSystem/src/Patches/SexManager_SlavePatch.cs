using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_SlavePatch
	{
		[HarmonyPatch(typeof(SexManager), "Slave")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(InventorySlot tmpSlave, ref IEnumerator __result)
		{
			var scene = new Slave(CommonUtils.GetActivePlayer(), tmpSlave);
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();
			return false;
		}
	}
}
