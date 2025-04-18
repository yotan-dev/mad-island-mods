using System.Collections;
using HFramework.Scenes;
using HarmonyLib;

namespace HFramework.Patches
{
	public class SexManager_DeliveryPatch
	{
		[HarmonyPatch(typeof(SexManager), "Delivery")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Delivery(CommonStates common, WorkPlace tmpWorkPlace, SexPlace tmpSexPlace, ref IEnumerator __result)
		{
			var scene = new Delivery(common, tmpWorkPlace, tmpSexPlace);
			__result = scene.Run();
			return false;
		}
	}
}
