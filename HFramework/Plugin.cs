using BepInEx;
using HFramework.Hook;
using HFramework.Patches;
using HFramework.Performer;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore.Events;
namespace HFramework
{
	
	[BepInPlugin("HFramework", "HFramework", "1.0.2")]
	[BepInDependency("YotanModCore", "1.5.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = Logger;

			HFramework.Config.Instance.Init(Config);

			if (HFramework.Config.Instance.ReplaceOriginalScenes.Value) {
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
				Harmony.CreateAndPatchAll(typeof(TranspilePlayerCheck));

				GameLifecycleEvents.OnGameStartEvent += () => { SexMeter.Instance.Reload(); };
				HookManager.RegisterHooksEvent += CommonHooks.Instance.InitHooks;
			}

			Harmony.CreateAndPatchAll(typeof(NpcMovePatches));

			PerformerLoader.Load();
			ScenesManager.Instance.Init();

			PLogger.LogInfo($"Plugin HFramework is loaded!");
		}
	}
}
