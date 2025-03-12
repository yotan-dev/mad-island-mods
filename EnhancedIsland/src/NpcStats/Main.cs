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

			Harmony.CreateAndPatchAll(typeof(NpcSpawnPatch));

			PLogger.LogInfo($"NpcStats is loaded!");
		}
	}
}
