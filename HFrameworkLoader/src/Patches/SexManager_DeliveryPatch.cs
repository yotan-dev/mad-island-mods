using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_DeliveryPatch
	{
		[HarmonyPatch(typeof(SexManager), "Delivery")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Delivery(CommonStates common, WorkPlace tmpWorkPlace, SexPlace tmpSexPlace, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_Delivery(common, tmpWorkPlace, tmpSexPlace, ref __result);
		}
	}
}
