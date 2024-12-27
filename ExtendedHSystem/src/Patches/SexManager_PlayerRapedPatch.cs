using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexManager_PlayerRapedPatch
	{
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_PlayerRaped(CommonStates to, CommonStates from, ref IEnumerator __result)
		{
			var scene = new PlayerRaped(to, from);
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();
			return false;
		}
	}
}
