using System.Collections;
using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexManager_CommonSexNPCPatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_CommonSexNPC(
			CommonStates npcA,
			CommonStates npcB,
			SexPlace sexPlace,
			ref IEnumerator __result
		)
		{
			return Plugin.Bridge.SexManager_Pre_CommonSexNPC(npcA, npcB, sexPlace, ref __result);
		}

		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_CommonSexNPC(
			IEnumerator __result
		)
		{
			PLogger.LogDebug("Post_SexManager_CommonSexNPC Start");

			while (__result.MoveNext())
				yield return __result.Current;

			PLogger.LogDebug("Post_SexManager_CommonSexNPC End");
		}


	}
}
