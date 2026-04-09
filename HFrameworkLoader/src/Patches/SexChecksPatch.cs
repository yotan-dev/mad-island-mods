using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class SexChecksPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.SexCheck))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_SexCheck(CommonStates from, CommonStates to, ref bool __result)
		{
			return Plugin.Bridge.SexManager_Pre_SexCheck(from, to, ref __result);
		}

		[HarmonyPatch(typeof(SexManager), nameof(SexManager.RapesCheck))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_RapesCheck(CommonStates from, CommonStates to, ref bool __result)
		{
			return Plugin.Bridge.SexManager_Pre_RapesCheck(from, to, ref __result);
		}

		[HarmonyPatch(typeof(NPCManager), nameof(NPCManager.LoveLoveCheck))]
		[HarmonyPostfix]
		private static bool Post_NPCManager_LoveLoveCheck(bool result, CommonStates commonA, CommonStates commonB)
		{
			return Plugin.Bridge.NPCManager_Post_LoveLoveCheck(result, commonA, commonB);
		}
	}
}
