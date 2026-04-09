using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_OnaniNPCPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.OnaniNPC))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_OnaniNPC(CommonStates common, SexPlace sexPlace, float upMoral, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_OnaniNPC(common, sexPlace, upMoral, ref __result);
		}
	}
}
