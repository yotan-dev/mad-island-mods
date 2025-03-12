using System;
using HarmonyLib;

namespace EnhancedIsland.DisassembleItems
{	
	public class Main
	{
		public void Init()
		{
			if (!PConfig.Instance.EnableDisassembleItems.Value)
				return;

			try {
				Harmony.CreateAndPatchAll(typeof(DisassembleItemsPatch));
				DisassembleTable.Init();

				PLogger.LogInfo($"Disassemble Items enabled");
			} catch (Exception e) {
				PLogger.LogError("Failed to enable Disassemble Items");
				PLogger.LogError(e);
			}
		}
	}
}
