using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace ExtendedHSystem.Patches
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
			scene.Init(new DefaultSceneController());
			__result = scene.Run();
			return false;
		}
	}
}
