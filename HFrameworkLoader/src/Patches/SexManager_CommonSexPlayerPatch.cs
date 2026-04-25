using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace HFrameworkLoader.Patches
{
	public class SexManager_CommonSexPlayerPatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_CommonSexPlayer(
			CommonStates pCommon,
			CommonStates nCommon,
			Vector3 pos,
			int sexType,
			ref IEnumerator __result
		)
		{
			return Plugin.Bridge.SexManager_Pre_CommonSexPlayer(pCommon, nCommon, pos, sexType, ref __result);
		}
	}
}
