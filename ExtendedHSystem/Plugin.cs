using System.IO;
using BepInEx;
using ExtendedHSystem.Hook;
using ExtendedHSystem.Patches;
using ExtendedHSystem.Performer;
using ExtendedHSystem.Scenes;
using ExtendedHSystem.ConfigFiles;
using HarmonyLib;
using Tomlyn;
using UnityEngine.UIElements.Collections;
using YotanModCore.Events;

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

				HookManager.RegisterHooksEvent += CommonHooks.Instance.InitHooks;
				
				if (ExtendedHSystem.Config.Instance.RequireForeplay.Enabled.Value)
					HookManager.RegisterHooksEvent += new Mods.RequireForeplay().InitHooks;

				if (ExtendedHSystem.Config.Instance.EnableDickPainter.Value)
					HookManager.RegisterHooksEvent += new Mods.DickPainter().InitHooks;
			}

			PerformerLoader.Load();
			// @TODO: Move this to a config file
			CommonSexPlayer.AddPerformer(PerformerLoader.Performers["EHS_Man_FemaleNative_Friendly_Normal"]);
			CommonSexPlayer.AddPerformer(PerformerLoader.Performers["EHS_Man_FemaleNative_Friendly_Pregnant"]);
			CommonSexPlayer.AddPerformer(PerformerLoader.Performers["EHS_Man_FemaleLargeNative_Friendly_Cowgirl_Normal"]);
			CommonSexPlayer.AddPerformer(PerformerLoader.Performers["EHS_Man_FemaleLargeNative_Friendly_Doggy_Normal"]);

			PLogger.LogInfo($"Plugin ExtendedHSystem is loaded!");
		}
	}
}
