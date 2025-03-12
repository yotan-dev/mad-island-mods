using HarmonyLib;

namespace EnhancedIsland.ItemColorInSlot
{
	
	public class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.EnableItemColorInSlot.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(ItemSlotPatch));

			PLogger.LogInfo($"Item Color in Slot is loaded!");
		}
	}
}
