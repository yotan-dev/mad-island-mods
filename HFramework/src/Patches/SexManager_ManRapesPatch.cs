using System.Collections;
using HFramework.Scenes;
using HarmonyLib;

namespace HFramework.Patches
{
	public class SexManager_ManRapesPatch
	{
		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_ManRapes(CommonStates girl, CommonStates man, ref IEnumerator __result)
		{
			var scene = new ManRapes(girl, man);
			__result = scene.Run();
			return false;
		}
	}
}
