using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_ToiletPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Toilet))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Toilet(InventorySlot tmpToile, ref IEnumerator __result)
		{
			var scene = new Toilet(CommonUtils.GetActivePlayer(), Managers.mn.inventory.itemSlot[50].common, tmpToile);
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();
			return false;
		}
	}
}
