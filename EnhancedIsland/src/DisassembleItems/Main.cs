using HarmonyLib;

namespace EnhancedIsland.DisassembleItems
{	
	public class Main
	{
		public void Init()
		{
			if (!PConfig.Instance.EnableDisassembleItems.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(DisassembleItemsPatch));
			DisassembleTable.Init();

			PLogger.LogInfo($"Disassemble Items enabled!");
		}
	}
}
