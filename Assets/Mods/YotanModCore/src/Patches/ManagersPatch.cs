using System.Collections.Generic;
using HarmonyLib;

namespace YotanModCore.Patches
{
	public class ManagersPatch
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
		}
	}
}