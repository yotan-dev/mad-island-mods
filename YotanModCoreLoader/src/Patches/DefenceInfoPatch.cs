using HarmonyLib;
using UnityEngine;
using YotanModCore.Items;

namespace YotanModCore.Patches
{
	internal static class DefenceInfoPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(DefenceInfo), nameof(DefenceInfo.DefenceDamage))]
		internal static bool Pre_DefenceInfo_DefenceDamage(DefenceInfo __instance, CommonStates attacker, float fixedDamage)
		{
			var skip = false;
			PLogger.LogDebug($"OnDefenceAttacked({attacker}, {fixedDamage})");

			// In co-op, defence damage is routed to the host; custom handling
			// must not mutate state on clients.
			if (__instance is CDefenceInfo cDefenceInfo && !(Coop.IsSupported && Coop.InNetworkScene(__instance) && Coop.IsClient()))
				skip = cDefenceInfo.OnDefenceAttacked(attacker, fixedDamage);

			return skip;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(DefenceInfo), "OnCollisionEnter")]
		internal static bool Pre_DefenceInfo_OnCollisionEnter(DefenceInfo __instance, Collision collision)
		{
			var skip = false;
			PLogger.LogDebug($"OnDefenceTouched({collision})");
			if (__instance is CDefenceInfo cDefenceInfo && !(Coop.IsSupported && Coop.InNetworkScene(__instance) && Coop.IsClient()))
				skip = cDefenceInfo.OnDefenceTouched(collision);

			return skip;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(DefenceInfo), nameof(DefenceInfo.Repair))]
		internal static void Post_DefenceInfo_Repair(DefenceInfo __instance)
		{
			PLogger.LogDebug($"OnDefenceRepaired()");

			// Co-op may suppress Repair() entirely; only notify when life was actually restored.
			if (__instance.itemInfo != null && __instance.itemInfo.life < __instance.itemInfo.lifeMax)
				return;

			if (__instance is CDefenceInfo cDefenceInfo)
				cDefenceInfo.OnDefenceRepaired();
		}
	}
}
