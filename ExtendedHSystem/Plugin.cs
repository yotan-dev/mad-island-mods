using BepInEx;
using ExtendedHSystem.Patches;
using HarmonyLib;

namespace ExtendedHSystem
{
	
	[BepInPlugin("ExtendedHSystem", "ExtendedHSystem", "0.1.0")]
	[BepInDependency("YotanModCore", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;

			ExtendedHSystem.Config.Instance.Init(Config);

			if (ExtendedHSystem.Config.Instance.ReplaceOriginalScenes.Value) {
				Harmony.CreateAndPatchAll(typeof(SexManager_AssWallPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_CommonSexNPCPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_CommonSexPlayerPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_DarumaPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_DeliveryPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_ManRapesPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_ManRapesSleepPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_OnaniNPCPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_PlayerRapedPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_SlavePatch));
			}

			PLogger.LogInfo($"Plugin ExtendedHSystem is loaded!");
		}
	}
}
