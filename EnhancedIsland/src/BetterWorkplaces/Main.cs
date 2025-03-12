using System;
using HarmonyLib;

namespace EnhancedIsland.BetterWorkplaces
{
	internal class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.Enabled.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(WorkResultPatch));

				PLogger.LogInfo($"Better Workplaces is enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable Better Workplaces");
				PLogger.LogError(e);
			}
		}
	}
}
