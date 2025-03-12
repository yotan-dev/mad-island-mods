using System;
using HarmonyLib;

namespace EnhancedIsland.StackNearby
{
	internal class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.EnableStackNearby.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(BuildInfoPatch));
				Harmony.CreateAndPatchAll(typeof(PlayerMovePatch));

				PLogger.LogInfo($"Stack Nearby is loaded!");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable stack nearby");
				PLogger.LogError(e);
			}
		}
	}
}
