using HarmonyLib;

namespace EnhancedIsland.NpcStats.Patches
{
	/// <summary>
	/// Additional patch for versions >= v0.4.4.3
	/// </summary>
	internal static class NpcSpawnPatch_0_4_4_3
	{
		[HarmonyPatch(typeof(NPCManager), nameof(NPCManager.NPCSetWorldLevel))]
		[HarmonyPostfix]
		private static void Post_NPCSetWorldLevel(CommonStates common)
		{
			StatDistributor.RedistributeStats(NpcKind.Enemy, common);
		}
	}
}
