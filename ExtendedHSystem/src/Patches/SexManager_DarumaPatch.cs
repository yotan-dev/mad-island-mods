using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_DarumaPatch
	{
		[HarmonyPatch(typeof(SexManager), "DarumaSex")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_DarumaSex(int state, InventorySlot tmpDaruma, ref IEnumerator __result)
		{
			var pCommon = CommonUtils.GetActivePlayer();
			var girlCommon = Managers.mn.inventory.itemSlot[50].common;
			
			var scene = new Daruma(pCommon, girlCommon, tmpDaruma);
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();

			return false;
		}
	}
}
