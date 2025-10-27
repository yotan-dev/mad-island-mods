using HarmonyLib;
using YotanModCore;

namespace YotanModCoreLoader.Patches
{
	internal static class LifecyclePatch
	{
		[HarmonyPatch(typeof(GameManager), "Start")]
		[HarmonyPrefix]
		private static void Pre_GamaManager_Start(GameManager __instance)
		{
			Plugin.ModCoreBridge.Pre_GameManager_Start(__instance);
		}

		[HarmonyPatch(typeof(SceneScript), "SceneChange")]
		[HarmonyPrefix]
		private static void Pre_SceneScript_SceneChange()
		{
			Plugin.ModCoreBridge.Pre_SceneScript_SceneChange();
		}
	}
}
