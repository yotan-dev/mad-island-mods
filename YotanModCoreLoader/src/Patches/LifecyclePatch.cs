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
			if (!__instance.IsGameScene())
				return;

			Plugin.InitializerResult.Pre_GameManager_Start?.Invoke(__instance);
		}

		[HarmonyPatch(typeof(SceneScript), "SceneChange")]
		[HarmonyPrefix]
		private static void Pre_SceneScript_SceneChange()
		{
			if (!Managers.mn.gameMN.IsGameScene())
				return;

			Plugin.InitializerResult.Pre_SceneScript_SceneChange?.Invoke();
		}
	}
}
