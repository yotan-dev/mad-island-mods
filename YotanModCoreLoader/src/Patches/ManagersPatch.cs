using HarmonyLib;
using YotanModCore.NpcTalk;

namespace YotanModCore.Patches
{
	public static class ManagersPatch
	{
		[HarmonyPatch(typeof(ManagersScript), "Awake")]
		[HarmonyPostfix]
		private static void Post_ManagerScript_Awake(ManagersScript __instance)
		{
			Managers.mn = __instance;
		}

		[HarmonyPatch(typeof(UIManager), "Awake")]
		[HarmonyPostfix]
		private static void Post_UIManager_Awake(UIManager __instance)
		{
			Managers.uiManager = new Wrappers.WrappedUIManager(__instance);
			Plugin.ModCoreBridge.Post_UIManager_Awake(__instance);
		}

		[HarmonyPatch(typeof(UIManager), nameof(UIManager.NPCPanelStateChange))]
		[HarmonyPostfix]
		private static void Post_UIManager_NPCPanelStateChange(CommonStates common)
		{
			Plugin.ModCoreBridge.Post_UIManager_NPCPanelStateChange(common);
		}
	}
}
