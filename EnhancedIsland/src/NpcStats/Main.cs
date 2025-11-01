using System;
using EnhancedIsland.NpcStats.Patches;
using HarmonyLib;
using YotanModCore;

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
				if (GameInfo.GameVersion >= GameInfo.ToVersion("0.4.4.3"))
					Harmony.CreateAndPatchAll(typeof(NpcSpawnPatch_0_4_4_3));
				else // < v0.4.4.3
					Harmony.CreateAndPatchAll(typeof(NpcSpawnPatch_Pre_0_4_4_3));

				PLogger.LogInfo($"Npc Stats enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable Npc Stats");
				PLogger.LogError(e);
			}
		}
	}
}
