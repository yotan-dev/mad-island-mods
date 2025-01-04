using System.Collections;
using System.Collections.Generic;
using HFramework.Hook;
using HFramework.ParamContainers;
using HFramework.Scenes;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework
{
	public class CommonHooks
	{
		public static readonly CommonHooks Instance = new CommonHooks();

		private Dictionary<IScene2, bool> ToiletCounted = new Dictionary<IScene2, bool>();

		private CommonHooks() { }

		public void InitHooks()
		{
			HookBuilder.New("HF.Friendly.OnPenetrate")
				.ForScenes(CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnPenetrate);
			HookBuilder.New("HF.Rape.OnPenetrate")
				.ForScenes(ManRapes.Name, ManRapesSleep.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnRapePenetrate);
			HookBuilder.New("HF.ManRapes.OnPenetrate")
				.ForScenes(ManRapes.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnManRapesPenetrate);
			HookBuilder.New("HF.Toilet.OnPenetrate")
				.ForScenes(AssWall.Name, Toilet.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnToiletsPenetrate);
			HookBuilder.New("HF.Masturbation.OnMasturbate")
				.ForScenes("*")
				.HookEvent(EventNames.OnMasturbate)
				.Call(this.OnMasturbate);

			HookBuilder.New("HF.Any.OnCreampie")
				.ForScenes("*")
				.HookEvent(EventNames.OnCreampie)
				.Call(this.OnCreampie);

			HookBuilder.New("HF.CommonSexPlayer.Affection")
				.ForScenes(CommonSexPlayer.Name)
				.HookStepEnd(CommonSexPlayer.StepNames.Main)
				.Call(this.OnCommonSexPlayerAffection);

			HookBuilder.New("HF.CommonSexNPC.Affection")
				.ForScenes(CommonSexNPC.Name)
				.HookStepEnd(CommonSexPlayer.StepNames.Main)
				.Call(this.OnCommonSexNPCAffection);

			HookBuilder.New("HF.PlayerRaped.Moral")
				.ForScenes(PlayerRaped.Name)
				.HookStepEnd(CommonSexPlayer.StepNames.Main)
				.Call(this.OnPlayerRapedMoral);

			HookBuilder.New("HF.OnEnd")
				.ForScenes(AssWall.Name)
				.HookStepEnd(AssWall.StepNames.Main)
				.Call(this.OnEnd);
		}

		private IEnumerator OnPenetrate(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.From, fromTo.Value.To, SexManager.SexCountState.Normal);
			yield break;
		}

		private IEnumerator OnRapePenetrate(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.From, fromTo.Value.To, SexManager.SexCountState.Rapes);
			yield break;
		}

		private IEnumerator OnToiletsPenetrate(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.From, fromTo.Value.To, SexManager.SexCountState.Toilet);
			yield break;
		}

		private IEnumerator OnMasturbate(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			fromTo.Value.From.sexInfo[SexInfoIndex.Masturbation]++;
			yield break;
		}

		private IEnumerator OnManRapesPenetrate(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			// Note: on original code, faint is only checked for Yona, Female Native and Native Girl,
			//       but doesn't make sense to check only for them... so we check for every NPC
			if (fromTo.Value.To.faint >= 0 && fromTo.Value.To.life >= 0)
				fromTo.Value.To.LoveChange(fromTo.Value.From, -10f, false);

			yield break;
		}

		private IEnumerator OnCreampie(IScene2 scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Creampie);
			yield break;
		}

		private IEnumerator OnCommonSexPlayerAffection(IScene2 scene, object param)
		{
			var commonSexPlayer = scene as CommonSexPlayer;
			if (commonSexPlayer == null)
				yield break;

			if (SexMeter.Instance.FillAmount == 1f)
				commonSexPlayer.Npc.LoveChange(commonSexPlayer.Player, 10f, false);
			else if (SexMeter.Instance.FillAmount < 0.3f)
				commonSexPlayer.Npc.LoveChange(commonSexPlayer.Player, -5f, false);

			yield break;
		}

		private IEnumerator OnCommonSexNPCAffection(IScene2 scene, object param)
		{
			var commonSexNpc = scene as CommonSexNPC;
			if (commonSexNpc == null)
				yield break;

			commonSexNpc.NpcB.LoveChange(commonSexNpc.NpcA, 10f, false);
			commonSexNpc.NpcA.LoveChange(commonSexNpc.NpcB, 10f, false);

			yield break;
		}

		private IEnumerator OnPlayerRapedMoral(IScene2 scene, object param)
		{
			var playerRaped = scene as PlayerRaped;
			if (playerRaped == null)
				yield break;

			if (playerRaped.Rapist.debuff.discontent == 4)
				playerRaped.Rapist.MoralChange(20f, null, NPCManager.MoralCause.None);
		}

		private IEnumerator OnEnd(IScene2 scene, object param)
		{
			if (this.ToiletCounted.ContainsKey(scene))
				this.ToiletCounted.Remove(scene);
			yield break;
		}
	}
}
