using HarmonyLib;

namespace EnhancedIsland.BetterWorkplaces
{
	internal class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.Enabled.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(WorkResultPatch));

			PLogger.LogInfo($"Better Workplaces is loaded!");
		}
	}
}
