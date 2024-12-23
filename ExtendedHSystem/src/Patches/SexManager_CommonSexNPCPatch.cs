using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_CommonSexNPCPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_CommonSexNPC(
			CommonStates npcA,
			CommonStates npcB,
			SexPlace sexPlace,
			SexManager.SexCountState sexType,
			ref IEnumerator __result
		)
		{
			var scene = new CommonSexNPC(npcA, npcB, sexPlace, sexType);
			scene.Init(DefaultSceneController);
			__result = scene.Run();
			return false;
		}
	}
}