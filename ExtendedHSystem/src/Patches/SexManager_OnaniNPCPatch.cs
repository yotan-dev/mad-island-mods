using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace ExtendedHSystem.Patches
{
	public class SexManager_OnaniNPCPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), nameof(SexManager.OnaniNPC))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(CommonStates common, SexPlace sexPlace, float upMoral, ref IEnumerator __result)
		{
			var scene = new OnaniNPC(common, sexPlace, upMoral);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}