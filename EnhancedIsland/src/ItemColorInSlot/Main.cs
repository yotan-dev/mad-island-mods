using System;
using HarmonyLib;

namespace EnhancedIsland.ItemColorInSlot
{
	
	public class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.EnableItemColorInSlot.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(ItemSlotPatch));

				PLogger.LogInfo($"Item Color in Slot is loaded!");
			} catch (Exception e) {
				PLogger.LogInfo("Failed to enable Item COlor in Slot");
				PLogger.LogError(e);
			}
		}
	}
}
