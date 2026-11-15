using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace EnhancedIsland.StackNearby
{
	public class PlayerMovePatch
	{
		[HarmonyPatch(typeof(PlayerMove), "Update")]
		[HarmonyPrefix]
		private static void Pre_PlayerMove_Update(PlayerMove __instance)
		{
			if (!__instance.controllable) {
				return;
			}

			if (Coop.IsSupported && Coop.BlocksInput(__instance)) {
				return;
			}

			if (Input.GetKeyDown(PConfig.Instance.StackNearbyKey.Value)) {
				StackNearbyController.StackNearby(__instance.transform.position);
			}
		}
	}
}
