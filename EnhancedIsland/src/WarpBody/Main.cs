using System;
using HarmonyLib;

namespace EnhancedIsland.WarpBody
{
	internal class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.EnableWarpBody.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(PlayerMovePatch));

				PLogger.LogInfo($"Warp Body is enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable Warp Body");
				PLogger.LogError(e);
			}
		}
	}
}
