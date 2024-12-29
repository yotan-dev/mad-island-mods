using BepInEx;
using HFramework.Hook;
using HFramework.Patches;
using HFramework.Performer;
using HFramework.Scenes;
using HarmonyLib;

namespace HFramework
{
	
	[BepInPlugin("HFramework", "HFramework", "0.2.0")]
	[BepInDependency("YotanModCore", "1.5.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;

			HFramework.Config.Instance.Init(Config);

			if (HFramework.Config.Instance.ReplaceOriginalScenes.Value) {
				// @TODO: Rework these scenes.
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
				Harmony.CreateAndPatchAll(typeof(SexManager_ToiletPatch));

				Harmony.CreateAndPatchAll(typeof(SexChecksPatch));

				HookManager.RegisterHooksEvent += CommonHooks.Instance.InitHooks;
			}

			PerformerLoader.Load();
			ScenesManager.Instance.Init();

			PLogger.LogInfo($"Plugin HFramework is loaded!");
		}
	}
}
