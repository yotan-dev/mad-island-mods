using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Hook;
using ExtendedHSystem.Performer;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace HExtensions.ExtendedScenes
{
	public class Main
	{
		public void Init()
		{
			Harmony.CreateAndPatchAll(typeof(Main));
			PerformerLoader.OnLoadPeformers += this.OnLoadPeformers;
			HookManager.RegisterHooksEvent += () => {
				HookBuilder.New("EHS.ExtendedScenes.ChangeAnim")
					.ForScenes(CommonSexPlayer.Name)
					.HookEvent(EventNames.OnAnimSetChanged)
					.Call(this.OnAnimSetChanged);
			};
		}

		private IEnumerator OnAnimSetChanged(IScene2 scene, object arg2)
		{
			if (scene is not CommonSexPlayer commonSexPlayer)
				yield break;

			var performer = (SexPerformer) arg2;
			if (performer.Info.Id == "EHS_Man_FemaleLargeNative_Friendly_Cowgirl_Normal")
				yield return commonSexPlayer.Idle();
		}

		private void OnLoadPeformers()
		{
			var performer = PerformerLoader.Performers.GetValueOrDefault("EHS_Man_FemaleLargeNative_Friendly_Cowgirl_Normal");
			if (performer == null)
			{
				PLogger.LogError("Failed to load performer EHS_Man_FemaleLargeNative_Friendly_Cowgirl_Normal");
				return;
			}

			var animSet = new AnimationSetBuilder("Anal");
			animSet.AddAnimation(ActionType.StartIdle, new ActionValue(PlayType.Loop, "A_idle", []));
			animSet.AddAnimation(ActionType.Caress, new ActionValue(PlayType.Loop, "Love_A_Contact_01", []));
			animSet.AddAnimation(ActionType.Insert, new ActionValue(PlayType.Once, "A_AttackToSex", ["OnPenetrate"]));
			animSet.AddAnimation(ActionType.Speed1, new ActionValue(PlayType.Loop, "A_Loop_01", []));
			animSet.AddAnimation(ActionType.Speed2, new ActionValue(PlayType.Loop, "A_Loop_02", []));
			animSet.AddAnimation(ActionType.Finish, new ActionValue(PlayType.Once, "A_Finish", ["OnOrgasm"]));
			animSet.AddAnimation(ActionType.FinishIdle, new ActionValue(PlayType.Loop, "A_Finish_idle", []));
			performer.AnimationSets.Add("Anal", animSet.Build());

			var defaultSet = performer.AnimationSets[SexPerformerInfo.DefaultSet];
			defaultSet.Actions.Add(new ActionKey(ActionType.StartIdle, 2), new ActionValue(PlayType.Loop, $"{SexPerformerInfo.AnimSetChangeName}:Anal:1", []));
			defaultSet.Actions.Add(new ActionKey(ActionType.Caress, 2), new ActionValue(PlayType.Loop, $"{SexPerformerInfo.AnimSetChangeName}:Anal:1", []));
			defaultSet.Actions.Add(new ActionKey(ActionType.Speed1, 2), new ActionValue(PlayType.Loop, $"{SexPerformerInfo.AnimSetChangeName}:Anal:1", []));
			defaultSet.Actions.Add(new ActionKey(ActionType.Speed2, 2), new ActionValue(PlayType.Loop, $"{SexPerformerInfo.AnimSetChangeName}:Anal:1", []));
			defaultSet.Actions.Add(new ActionKey(ActionType.FinishIdle, 2), new ActionValue(PlayType.Loop, $"{SexPerformerInfo.AnimSetChangeName}:Anal:1", []));
		}

		[HarmonyPatch(typeof(SexPerformer), nameof(SexPerformer.GetAlternativePoseName))]
		[HarmonyPrefix]
		private static bool SexPerformer_Pre_GetAlternativePoseName(SexPerformer __instance, out string __result)
		{
			if (!__instance.HasAlternativePose())
			{
				__result = null;
				return true;
			}

			if (__instance.Info.Id == "EHS_Man_FemaleLargeNative_Friendly_Cowgirl_Normal")
			{
				__result = __instance.CurrentSetName == "Anal" ? "Pos: Vaginal": "Pos: Anal";
				return false;
			}

			__result = "";
			return true;
		}
	}
}
