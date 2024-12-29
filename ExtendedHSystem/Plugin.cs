using BepInEx;
using ExtendedHSystem.Hook;
using ExtendedHSystem.Patches;
using ExtendedHSystem.Performer;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace ExtendedHSystem
{
	
	[BepInPlugin("ExtendedHSystem", "ExtendedHSystem", "0.2.0")]
	[BepInDependency("YotanModCore", "1.5.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;

			ExtendedHSystem.Config.Instance.Init(Config);

			if (ExtendedHSystem.Config.Instance.ReplaceOriginalScenes.Value) {
				// @TODO: Rework these scenes.
				// Harmony.CreateAndPatchAll(typeof(SexManager_AssWallPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_CommonSexNPCPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_CommonSexPlayerPatch));
				// Harmony.CreateAndPatchAll(typeof(SexManager_DarumaPatch));
				// Harmony.CreateAndPatchAll(typeof(SexManager_DeliveryPatch));
				// Harmony.CreateAndPatchAll(typeof(SexManager_ManRapesPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_ManRapesSleepPatch));
				// Harmony.CreateAndPatchAll(typeof(SexManager_OnaniNPCPatch));
				Harmony.CreateAndPatchAll(typeof(SexManager_PlayerRapedPatch));
				// Harmony.CreateAndPatchAll(typeof(SexManager_SlavePatch));
				// Harmony.CreateAndPatchAll(typeof(SexManager_ToiletPatch));

				Harmony.CreateAndPatchAll(typeof(SexChecksPatch));

				HookManager.RegisterHooksEvent += CommonHooks.Instance.InitHooks;
			}

			PerformerLoader.Load();
			ScenesManager.Instance.Init();

			PLogger.LogInfo($"Plugin ExtendedHSystem is loaded!");
		}
	}
}
