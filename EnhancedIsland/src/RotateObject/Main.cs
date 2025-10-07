using System;
using HarmonyLib;

namespace EnhancedIsland.RotateObject
{
	internal class Main
	{
		internal void Init()
		{
			// if (!PConfig.Instance.Enabled.Value)
			// 	return;

			try {
				Harmony.CreateAndPatchAll(typeof(TranspileBuildStart));
				Harmony.CreateAndPatchAll(typeof(TranspileBuildMove));

				PLogger.LogInfo($"Rotate Object is enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable Rotate Object");
				PLogger.LogError(e);
			}
		}
	}
}
