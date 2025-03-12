using HarmonyLib;

namespace EnhancedIsland.StackNearby
{
	internal class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.EnableStackNearby.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(BuildInfoPatch));
			Harmony.CreateAndPatchAll(typeof(PlayerMovePatch));

			PLogger.LogInfo($"Stack Nearby is loaded!");
		}
	}
}
