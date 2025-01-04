using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
{
	public class SexManager_SlavePatch
	{
		[HarmonyPatch(typeof(SexManager), "Slave")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(InventorySlot tmpSlave, ref IEnumerator __result)
		{
			var scene = new Slave(CommonUtils.GetActivePlayer(), tmpSlave);
			scene.Init(new DefaultSceneController());
			__result = scene.Run();
			return false;
		}
	}
}
