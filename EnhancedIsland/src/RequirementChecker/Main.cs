using HarmonyLib;

namespace EnhancedIsland.RequirementChecker
{
	public class Main
	{
		public void Init()
		{
			if (!PConfig.Instance.EnableRequirementChecker.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(ItemDescriptionPatch));

			PLogger.LogInfo($"Requirement Checker is loaded!");
		}
	}
}
