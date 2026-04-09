using BepInEx;
using HarmonyLib;
using HFramework;
using HFrameworkLoader.Patches;

namespace HFrameworkLoader
{
	[BepInPlugin("HFramework", "HFramework", MyPluginInfo.PLUGIN_VERSION)]
	[BepInDependency("YotanModCore", "2.2.7")]
	public class Plugin : BaseUnityPlugin
	{
#nullable disable // Awake will set it. Too much effort to type this as nullable
		internal static HFrameworkBridge Bridge { get; private set; }
#nullable enable

		private bool Loaded = false;

		private int Ticks = 0;

		private void Awake() {
			HFrameworkLoader.Config.Instance.Init(Config);

			Plugin.Bridge = Initializer.Init(new BepisLogger(Logger));
			PLogger._Logger = Logger;

			var hfConfig = HFConfig.GetInstance();
			if (hfConfig.ReplaceOriginalScenes) {
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
			}

			Harmony.CreateAndPatchAll(typeof(NpcMovePatches));

			PLogger.LogInfo($"Plugin HFramework is loaded!");
		}

		private void Update() {
			if (this.Loaded)
				return;

			Ticks++;

			if (this.Ticks > 5) {
				this.Loaded = true;
				Plugin.Bridge.AfterStartTicks();
			}
		}
	}
}
