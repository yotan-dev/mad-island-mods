using HarmonyLib;
using YotanModCore;

namespace YoUnnoficialPatches.Patches
{
	/**
	 * Prevents NPCs from sexing when there are no scenes
	 */
	public static class DontStartInvalidSexPatch
	{
		[HarmonyPatch(typeof(NPCManager), nameof(NPCManager.ShallWeSex))]
		[HarmonyPostfix]
		private static void Post_NPCManager_ShallWeSex(CommonStates commonA, CommonStates commonB, ref SexManager.SexInviteState __result)
		{
			if (__result != SexManager.SexInviteState.Yes)
				return;

			if (!Managers.mn.sexMN.SexCheck(commonA, commonB))
				__result = SexManager.SexInviteState.Cant;
		}
	}
}
