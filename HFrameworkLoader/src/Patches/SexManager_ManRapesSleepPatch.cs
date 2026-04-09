using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_ManRapesSleepPatch
	{
		[HarmonyPatch(typeof(SexManager), "ManRapesSleep")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_ManRapesSleep(CommonStates girl, CommonStates man, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_ManRapesSleep(girl, man, ref __result);
		}
	}
}
