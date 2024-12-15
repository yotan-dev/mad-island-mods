using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace ExtendedHSystem.Patches
{
	public class SexManager_ManRapesPatch
	{
		private static readonly ISceneController DefaultSceneController = new DefaultSceneController();

		private static readonly SceneEventHandler DefaultSceneEventHandler = new DefaultSceneEventHandler();

		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_ManRapes(CommonStates girl, CommonStates man, ref IEnumerator __result)
		{
			var scene = new ManRapes(girl, man);
			scene.Init(DefaultSceneController);
			scene.AddEventHandler(DefaultSceneEventHandler);
			__result = scene.Run();
			return false;
		}
	}
}