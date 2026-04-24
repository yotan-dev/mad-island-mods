using HarmonyLib;
using UnityEngine;

namespace YoUnnoficialPatches.Patches
{
	/**
	 * Prevents keys from affect the game world while debug tool is open.
	 * For example, if you have a NPC selected and start typing a command with "a",
	 * the select NPC menu does not close because you are walking.
	 *
	 * This is achieved by patching Unity's Input.GetKeyDown and Input.GetKey,
	 * so there might be unknown side effects.
	 */
	public static class FixDebugToolInputPatch
	{
		private static DebugTool _debugTool;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(DebugTool), nameof(DebugTool.Start))]
		internal static void Post_DebugTool_Start(DebugTool __instance)
		{
			_debugTool = __instance;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), new[] { typeof(KeyCode) })]
		internal static bool Pre_Input_GetKeyDown(ref bool __result, KeyCode key)
		{
			if ((_debugTool?.debugCanvas?.activeSelf ?? false) && key != KeyCode.Return)
			{
				__result = false;
				return false;
			}

			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(Input), nameof(Input.GetKey), new[] { typeof(KeyCode) })]
		internal static bool Pre_Input_GetKey(ref bool __result, KeyCode key)
		{
			if ((_debugTool?.debugCanvas?.activeSelf ?? false) && key != KeyCode.Return)
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}
