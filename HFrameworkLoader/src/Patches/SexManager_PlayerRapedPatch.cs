using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_PlayerRapedPatch
	{
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_PlayerRaped(CommonStates to, CommonStates from, ref IEnumerator __result)
		{
			return Plugin.Bridge.SexManager_Pre_PlayerRaped(to, from, ref __result);
		}
	}
}
