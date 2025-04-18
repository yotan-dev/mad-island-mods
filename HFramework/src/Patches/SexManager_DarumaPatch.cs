using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
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
			__result = scene.Run();

			return false;
		}
	}
}
