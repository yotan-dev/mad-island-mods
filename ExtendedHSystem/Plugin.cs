using BepInEx;
using ExtendedHSystem.Hook;
using ExtendedHSystem.Patches;
using ExtendedHSystem.Performer;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;
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

			GameLifecycleEvents.OnGameStartEvent += () =>
			{
				CommonSexPlayer.AddPerformer(
					new SexPerformerInfoBuilder("EHS_Man_FemaleNative_Friendly_Normal")
						.SetSexPrefab(Managers.mn.sexMN.sexList[1].sexObj[1])
						.SetActors(NpcID.Man /* 0 */, NpcID.FemaleNative /* 15 */)
						.AddCondition(new PregnantCheck(NpcID.FemaleNative, false))
						.AddAnimation(ActionType.StartIdle, new ActionValue(PlayType.Loop, "A_idle"))
						.AddAnimation(ActionType.Caress, new ActionValue(PlayType.Loop, "A_Contact_01_<Tits>"))
						// Sex pose 1
						.AddAnimation(ActionType.Insert, 1, new ActionValue(PlayType.Loop, "A_Loop_01"))
						.AddAnimation(ActionType.Speed, 1, new ActionValue(PlayType.Loop, "A_Loop_02"))
						.AddAnimation(ActionType.Finish, 1, new ActionValue(PlayType.Once, "A_Finish"))
						.AddAnimation(ActionType.FinishIdle, 1, new ActionValue(PlayType.Loop, "A_Finish_idle"))
						// Sex pose 2
						.AddAnimation(ActionType.Speed, 2, new ActionValue(PlayType.Loop, "A_Loop_02_<Tits>"))
						.AddAnimation(ActionType.Finish, 2, new ActionValue(PlayType.Once, "A_Finish_00"))
						.AddAnimation(ActionType.FinishIdle, 2, new ActionValue(PlayType.Loop, "A_Finish_00_idle"))
						.Build()
				);
				CommonSexPlayer.AddPerformer(
					new SexPerformerInfoBuilder("EHS_Man_FemaleNative_Friendly_Pregnant")
						.SetSexPrefab(Managers.mn.sexMN.sexList[1].sexObj[20])
						.SetActors(NpcID.Man /* 0 */, NpcID.FemaleNative /* 15 */)
						.AddCondition(new PregnantCheck(NpcID.FemaleNative, true))
						.AddAnimation(ActionType.StartIdle, new ActionValue(PlayType.Loop, "A_idle"))
						.AddAnimation(ActionType.Caress, new ActionValue(PlayType.Loop, "A_Contact_01_<Tits>"))
						.AddAnimation(ActionType.Insert, new ActionValue(PlayType.Loop, "A_Loop_01"))
						.AddAnimation(ActionType.Speed, new ActionValue(PlayType.Loop, "A_Loop_02"))
						.AddAnimation(ActionType.Finish, new ActionValue(PlayType.Once, "A_Finish"))
						.AddAnimation(ActionType.FinishIdle, new ActionValue(PlayType.Loop, "A_Finish_idle"))
						.Build()
				);
				CommonSexPlayer.AddPerformer(
					new SexPerformerInfoBuilder("EHS_Man_FemaleLargeNative_Friendly_Cowgirl_Normal")
						.SetSexPrefab(Managers.mn.sexMN.sexList[8].sexObj[1])
						.SetActors(NpcID.Man /* 0 */, NpcID.FemaleLargeNative /* 17 */)
						.AddCondition(new SexTypeCheck(0))
						.AddAnimation(ActionType.StartIdle, new ActionValue(PlayType.Loop, "Love_A_idle"))
						.AddAnimation(ActionType.Caress, new ActionValue(PlayType.Loop, "Love_A_Contact_01"))
						.AddAnimation(ActionType.Insert, new ActionValue(PlayType.Loop, "Love_A_Loop_01"))
						.AddAnimation(ActionType.Speed, new ActionValue(PlayType.Loop, "Love_A_Loop_02"))
						.AddAnimation(ActionType.Finish, new ActionValue(PlayType.Once, "Love_A_Finish"))
						.AddAnimation(ActionType.FinishIdle, new ActionValue(PlayType.Loop, "Love_A_Finish_idle"))
						.Build()
				);
				CommonSexPlayer.AddPerformer(
					new SexPerformerInfoBuilder("EHS_Man_FemaleLargeNative_Friendly_Doggy_Normal")
						.SetSexPrefab(Managers.mn.sexMN.sexList[8].sexObj[15])
						.SetActors(NpcID.Man /* 0 */, NpcID.FemaleLargeNative /* 17 */)
						.AddCondition(new SexTypeCheck(1))
						.AddAnimation(ActionType.StartIdle, new ActionValue(PlayType.Loop, "Love_A_idle"))
						.AddAnimation(ActionType.Caress, new ActionValue(PlayType.Loop, "Love_A_Contact_01"))
						.AddAnimation(ActionType.Insert, new ActionValue(PlayType.Loop, "Love_A_Loop_01"))
						.AddAnimation(ActionType.Speed, new ActionValue(PlayType.Loop, "Love_A_Loop_02"))
						.AddAnimation(ActionType.Finish, new ActionValue(PlayType.Once, "Love_A_Finish"))
						.AddAnimation(ActionType.FinishIdle, new ActionValue(PlayType.Loop, "Love_A_Finish_idle"))
						.Build()
				);
			};

			PLogger.LogInfo($"Plugin ExtendedHSystem is loaded!");
		}
	}
}
