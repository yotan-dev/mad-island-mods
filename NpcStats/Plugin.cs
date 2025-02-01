using BepInEx;
using HarmonyLib;
using NpcStats.Config;
using NpcStats.Patches;

namespace NpcStats
{
	[BepInPlugin("NpcStats", "NpcStats", "3.0.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;
			PConfig.Instance.Init(this.Config);

			Harmony.CreateAndPatchAll(typeof(NpcSpawnPatch));

			PLogger.LogInfo($"Plugin NpcStats is loaded!");
		}
	}
}
