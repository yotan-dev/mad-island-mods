using HarmonyLib;

namespace HFrameworkLoader.Patches
{
	public class NpcMovePatches
	{
		[HarmonyPatch(typeof(NPCMove), nameof(NPCMove.Wait))]
		[HarmonyPrefix]
		private static void NPCMove_Wait(NPCMove __instance)
		{
			Plugin.Bridge.NPCMove_Pre_Wait(__instance);
		}
	}
}
