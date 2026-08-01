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
		private static void Post_NPCManager_ShallWeSex(CommonStates from, CommonStates to, ref SexManager.SexInviteState __result)
		{
			if (__result != SexManager.SexInviteState.Yes)
				return;

			// If from is not friend, SexCheck will always fail
			// I don't know why SexCheck checks if from is friend, but it does and I don't know what could happen if I remove that.
			// Running SexCheck in those cases would make Herb Village fail, because we have non-friend NPCs trying to sex with perfume there.
			// So, just ignore non friends, as they already work quite well, I think.
			if (!Managers.mn.npcMN.IsActiveFriend(to))
				return;

			if (!Managers.mn.sexMN.SexCheck(from, to))
				__result = SexManager.SexInviteState.Cant;
		}
	}
}
