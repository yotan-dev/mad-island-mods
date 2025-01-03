using System.Collections;
using HFramework.Scenes;
using HarmonyLib;

namespace HFramework.Patches
{
	public class SexManager_PlayerRapedPatch
	{
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_PlayerRaped(CommonStates to, CommonStates from, ref IEnumerator __result)
		{
			var scene = new PlayerRaped(to, from);
			scene.Init(new DefaultSceneController());
			__result = scene.Run();
			return false;
		}
	}
}
