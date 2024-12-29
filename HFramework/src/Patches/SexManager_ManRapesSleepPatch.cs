using System.Collections;
using HFramework.Scenes;
using HarmonyLib;

namespace HFramework.Patches
{
	public class SexManager_ManRapesSleepPatch
	{
		[HarmonyPatch(typeof(SexManager), "ManRapesSleep")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_ManRapesSleep(CommonStates girl, CommonStates man, ref IEnumerator __result)
		{
			var scene = new ManRapesSleep(girl, man);
			scene.Init(new DefaultSceneController());
			__result = scene.Run();
			return false;
		}
	}
}
