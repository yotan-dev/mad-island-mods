using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_ManRapesPatch
	{
		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_ManRapes(CommonStates girl, CommonStates man, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_ManRapes(girl, man, ref __result);
		}
	}
}
