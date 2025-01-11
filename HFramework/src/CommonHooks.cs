using System;
using System.Collections;
using System.Collections.Generic;
using HFramework.Handlers;
using HFramework.Hook;
using HFramework.ParamContainers;
using HFramework.Scenes;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework
{
	public class CommonHooks
	{
		public static readonly CommonHooks Instance = new CommonHooks();

		private Dictionary<IScene, bool> ToiletCounted = new Dictionary<IScene, bool>();

		private CommonHooks() { }

		public void InitHooks()
		{
			HookBuilder.New("HF.Friendly.OnPenetrate")
				.ForScenes(CommonSexNPC.Name, CommonSexPlayer.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.CountNormal);
			HookBuilder.New("Gallery.Friendly.OnScissor")
				.ForScenes(CommonSexNPC.Name)
				.HookEvent(EventNames.OnScissor)
				.Call(this.CountNormal);
			HookBuilder.New("HF.Rape.OnPenetrate")
				.ForScenes(Daruma.Name, ManRapes.Name, ManRapesSleep.Name, Slave.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnRapePenetrate);
			HookBuilder.New("HF.PlayerRaped.OnPenetrate")
				.ForScenes(PlayerRaped.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnPlayerRapedPenetrate);
			HookBuilder.New("HF.ManRapes.OnPenetrate")
				.ForScenes(ManRapes.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnManRapesPenetrate);
			HookBuilder.New("HF.Toilet.OnPenetrate")
				.ForScenes(AssWall.Name, Toilet.Name)
				.HookEvent(EventNames.OnPenetrate)
				.Call(this.OnToiletsPenetrate);

			HookBuilder.New("HF.Delivery.OnDelivery")
				.ForScenes(Delivery.Name)
				.HookEvent(EventNames.OnDelivery)
				.Call(this.OnDelivery);


			HookBuilder.New("HF.PlayerRaped.OnDefeated")
				.ForScenes(PlayerRaped.Name)
				.HookEvent(EventNames.OnPlayerDefeated)
				.Call(this.OnPlayerRapedDefeated);


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

		private IEnumerator OnPlayerRapedDefeated(IScene scene, object arg2)
		{
			Managers.mn.uiMN.MainCanvasView(false);
			yield return Managers.mn.sexMN.StartCoroutine(Managers.mn.sound.GoBGMFade(1));
			GameObject.Find("UIFXPool").transform.Find("ReviveSlider").GetComponent<Slider>().gameObject.SetActive(false);
			yield return new WaitForSeconds(1f);
			Managers.mn.uiMN.SkipView(true);
		}

		private IEnumerator CountNormal(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Normal);
			yield break;
		}

		private IEnumerator OnRapePenetrate(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Rapes);
			yield break;
		}

		private IEnumerator OnPlayerRapedPenetrate(IScene scene, object param)
		{
			if (!(scene is PlayerRaped playerRaped))
				yield break;

			Managers.mn.sexMN.SexCountChange(playerRaped.Player, playerRaped.Rapist, SexManager.SexCountState.Rapes);
			yield break;
		}

		private IEnumerator OnToiletsPenetrate(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			// Official code counts toilet once for AssWall but several times for Toilet, which is inconsistent.
			// We count once always, as IMO it means the start of the "interaction", not how many times you did
			if (ToiletCounted.ContainsKey(scene))
				yield break;

			ToiletCounted.Add(scene, true);
			Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Toilet);
			yield break;
		}

		private IEnumerator OnDelivery(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			if (scene is not Delivery delivery)
				yield break;

			var mother = fromTo.Value.From;
			if (!CommonUtils.IsPregnant(mother))
				yield break;

			yield return new SpawnChild(scene, mother, delivery.SuccessRate).Handle();

			Managers.mn.sexMN.SexCountChange(mother, null, SexManager.SexCountState.Delivery);
			mother.age++;

			Managers.mn.sexMN.Pregnancy(mother, null, false);
		}

		private IEnumerator OnMasturbate(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			fromTo.Value.From.sexInfo[SexInfoIndex.Masturbation]++;
			yield break;
		}

		private IEnumerator OnManRapesPenetrate(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue || scene is not ManRapes manRapes)
				yield break;

			// Note: on original code, faint is only checked for Yona, Female Native and Native Girl,
			//       but doesn't make sense to check only for them... so we check for every NPC
			if (manRapes.InitialFaint > 0 && manRapes.InitialLife > 0)
				fromTo.Value.To.LoveChange(fromTo.Value.From, -10f, false);

			yield break;
		}

		private IEnumerator OnCreampie(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue)
				yield break;

			Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Creampie);
			yield break;
		}

		private IEnumerator OnCommonSexPlayerAffection(IScene scene, object param)
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

		private IEnumerator OnCommonSexNPCAffection(IScene scene, object param)
		{
			var commonSexNpc = scene as CommonSexNPC;
			if (commonSexNpc == null || !commonSexNpc.Success)
				yield break;

			commonSexNpc.Npc1.LoveChange(commonSexNpc.Npc2, 10f, false);
			commonSexNpc.Npc2.LoveChange(commonSexNpc.Npc1, 10f, false);

			yield break;
		}

		private IEnumerator OnPlayerRapedMoral(IScene scene, object param)
		{
			var playerRaped = scene as PlayerRaped;
			if (playerRaped == null)
				yield break;

			if (playerRaped.Rapist.debuff.discontent == 4)
				playerRaped.Rapist.MoralChange(20f, null, NPCManager.MoralCause.None);
		}

		private IEnumerator OnEnd(IScene scene, object param)
		{
			if (this.ToiletCounted.ContainsKey(scene))
				this.ToiletCounted.Remove(scene);
			yield break;
		}
	}
}
