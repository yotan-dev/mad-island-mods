using HarmonyLib;
using UnityEngine;

namespace StackNearby
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

			if (Input.GetKeyDown(KeyCode.V)) {
				StackNearbyController.StackNearby(__instance.transform.position);
			}
		}
	}
}