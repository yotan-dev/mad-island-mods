using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace ExtendedHSystem.Patches
{
	public class SexManager_ManRapesSleepPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), "ManRapesSleep")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_ManRapesSleep(CommonStates girl, CommonStates man, ref IEnumerator __result)
		{
			var scene = new ManRapesSleep(girl, man);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}