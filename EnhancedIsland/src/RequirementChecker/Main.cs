using System;
using HarmonyLib;

namespace EnhancedIsland.RequirementChecker
{
	public class Main
	{
		public void Init()
		{
			if (!PConfig.Instance.EnableRequirementChecker.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(ItemDescriptionPatch));

				PLogger.LogInfo("Requirement Checker is enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to load Requirement Checker");
				PLogger.LogError(e);
			}
		}
	}
}
