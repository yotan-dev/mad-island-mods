using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
{
	[HarmonyPatch]
	public static class TranspilePlayerCheck
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			var result = new List<MethodBase>
			{
				AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(NPCMove), nameof(NPCMove.Live)))
			};
			return result;
		}

		/// <summary>
		/// Checks whether st can sex with a NPC.
		/// It should return > 1 if it can (currently, returns 999)
		/// </summary>
		/// <param name="st"></param>
		/// <returns></returns>
		private static int CanSexWithNpc(CommonStates st)
		{
			// We allow player characters to do it, as long as they are not the active player
			// otherwise it would block player control.
			// This is being controlled by the sex scenes anyway, and in the default game it would
			// never happen.
			int ret = 0;
			if (st.npcID != CommonUtils.GetActivePlayer().npcID)
				ret = 999;

			return ret;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			/*
			* Looks for:
				CommonStates partner = NPCManager.GetSexableLover(this.common);
				if (partner != null && partner.npcID > 1) {
					// ...

			* Replace with:
				CommonStates partner = NPCManager.GetSexableLover(this.common);
				if (partner != null && CanSexWithNpc(partner) > 1) {
					
				}	
			*/

			var codeMatcher = new CodeMatcher(instructions);
			codeMatcher
				.MatchForward(
					true,
					new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(NPCManager), nameof(NPCManager.GetSexableLover)))
				)
				.ThrowIfInvalid("Could not find call to GetSexableLover")
				.MatchForward(
					true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldfld),
					new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CommonStates), nameof(CommonStates.npcID)))
				)
				.ThrowIfInvalid("Could not find field npcID")
				.RemoveInstruction()
				.InsertAndAdvance(
					new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TranspilePlayerCheck), nameof(TranspilePlayerCheck.CanSexWithNpc)))
				);

			return codeMatcher.Instructions();
		}
	}
}
