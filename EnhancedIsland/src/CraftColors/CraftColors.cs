using HarmonyLib;

namespace EnhancedIsland.CraftColors
{
	public class CraftColors
	{
		public void Init()
		{
			if (!PConfig.Instance.EnableCraftColors.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(ItemDescriptionPatch));
		}
	}
}