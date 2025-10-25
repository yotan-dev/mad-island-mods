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
			if (__instance is CDefenceInfo cDefenceInfo)
				skip = cDefenceInfo.OnDefenceAttacked(attacker, fixedDamage);

			return skip;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(DefenceInfo), "OnCollisionEnter")]
		internal static bool Pre_DefenceInfo_OnCollisionEnter(DefenceInfo __instance, Collision collision)
		{
			var skip = false;
			PLogger.LogDebug($"OnDefenceTouched({collision})");
			if (__instance is CDefenceInfo cDefenceInfo)
				skip = cDefenceInfo.OnDefenceTouched(collision);

			return skip;
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(DefenceInfo), nameof(DefenceInfo.Repair))]
		internal static void Post_DefenceInfo_Repair(DefenceInfo __instance)
		{
			PLogger.LogDebug($"OnDefenceRepaired()");
			if (__instance is CDefenceInfo cDefenceInfo)
				cDefenceInfo.OnDefenceRepaired();
		}
	}
}
