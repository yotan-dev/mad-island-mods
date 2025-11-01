using HarmonyLib;

namespace EnhancedIsland.NpcStats.Patches
{
	/// <summary>
	/// Additional patch for versions prior to v0.4.4.3
	/// </summary>
	internal static class NpcSpawnPatch_Pre_0_4_4_3
	{
		[HarmonyPatch(typeof(NPCManager), "NPCLevelSet")]
		[HarmonyPostfix]
		private static void Post_NPCLevelSet(CommonStates common)
		{
			StatDistributor.RedistributeStats(NpcKind.Enemy, common);
		}
	}
}
