using HarmonyLib;

namespace EnhancedIsland.RequirementChecker
{
	public class RequirementChecker
	{
		public void Init()
		{
			if (!PConfig.Instance.EnableRequirementChecker.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(ItemDescriptionPatch));
		}
	}
}