using System;
using System.Collections;
using System.Collections.Generic;
using HFramework.Scenes;
using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace HFramework.Patches
{
	public class SexManager_CommonSexPlayerPatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_CommonSexPlayer(
			int state,
			CommonStates pCommon,
			CommonStates nCommon,
			Vector3 pos,
			int sexType,
			ref IEnumerator __result
		)
		{
			var scene = new CommonSexPlayer(pCommon, nCommon, pos, sexType);
			__result = scene.Run();
			return false;
		}
	}
}
