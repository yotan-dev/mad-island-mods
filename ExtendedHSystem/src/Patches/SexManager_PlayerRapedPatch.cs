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
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_PlayerRaped(CommonStates to, CommonStates from, ref IEnumerator __result)
		{
			var scene = new PlayerRaped(to, from);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}