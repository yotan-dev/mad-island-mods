using System;
using EnhancedIsland.NpcStats.Patches;
using HarmonyLib;

namespace EnhancedIsland.NpcStats
{
	public class Main
	{
		public void Init()
		{
			if (!Config.PConfig.Instance.Enabled.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(NpcSpawnPatch));

				PLogger.LogInfo($"Npc Stats enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable Npc Stats");
				PLogger.LogError(e);
			}
		}
	}
}
