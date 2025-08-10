using System;
using System.Collections;
using System.Collections.Generic;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
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
			var scene = new CommonSexNPC(npcA, npcB, sexPlace);
			__result = scene.Run();
			return false;
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
