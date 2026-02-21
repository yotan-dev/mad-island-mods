using System;
using System.Linq;
using HFramework.Scenes;
using HFramework.SexScripts;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework
{
	internal class DefaultSexEventHandler
	{
		internal static DefaultSexEventHandler Instance { get; private set; } = new DefaultSexEventHandler();

		public void Init()
		{
			SexEvents.OnPerformHandJob.Triggered += this.OnSexStart;
			SexEvents.OnPerformTitFuck.Triggered += this.OnSexStart;
			SexEvents.OnPerformScissor.Triggered += this.OnSexStart;
			SexEvents.OnPerformMasturbation.Triggered += this.OnMasturbation;
			SexEvents.OnPerformDelivery.Triggered += this.OnDelivery;

			SexEvents.OnPenetrateVagina.Triggered += this.OnSexStart;
			SexEvents.OnPenetrateMouth.Triggered += this.OnSexStart;
			SexEvents.OnPenetrateAss.Triggered += this.OnSexStart;

			// SexEvents.OnLickVagina.Triggered += this.OnSexStart;

			// SexEvents.OnOrgasm.Triggered += this.OnSexStart;
			SexEvents.OnCumOnVagina.Triggered += this.OnCumOnVagina;
			// SexEvents.OnCumOnAss.Triggered += this.OnSexStart;
			// SexEvents.OnCumOnMouth.Triggered += this.OnSexStart;
			// SexEvents.OnCumOnTits.Triggered += this.OnSexStart;

			// SexEvents.OnGiveBirth.Triggered += this.OnSexStart;
			// SexEvents.OnStillbirth.Triggered += this.OnSexStart;

			// @TODO:
			// SexEvents.OnPlayerDefeated.Triggered += this.OnPlayerDefeated;

			SexEvents.OnEnd.Triggered += this.OnSexEnd;
		}

		private void OnCumOnVagina(object sender, FromToEventArgs e)
		{
			// Added to debug a reported crash in discord.
			PLogger.LogDebug($"OnCumOnVagina: {e.From?.charaName ?? "NULL"} -> {e.To?.charaName ?? "NULL"}");

			Managers.mn.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Creampie);
		}

		private void OnSexStart(object sender, FromToEventArgs e)
		{
			if (e.From == null || e.To == null)
				return;

			if (e.isRape)
			{
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Rapes);

				// @TODO:
				// if (e.ctx is ManRapes) {
				// 	// Note: on original code, faint is only checked for Yona, Female Native and Native Girl,
				// 	//       but doesn't make sense to check only for them... so we check for every NPC
				// 	if (manRapes.InitialFaint > 0 && manRapes.InitialLife > 0)
				// 		fromTo.Value.To.LoveChange(fromTo.Value.From, -10f, false);
				// }
			}
			else
			{
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Normal);
			}

			// @TODO:
			// if (e.ctx is Toilet || e.ctx is AssWall) {
			// 	// Official code counts toilet once for AssWall but several times for Toilet, which is inconsistent.
			// 	// We count once always, as IMO it means the start of the "interaction", not how many times you did
			// 	if (ToiletCounted.ContainsKey(scene))
			// 		yield break;

			// 	ToiletCounted.Add(scene, true);
			// 	Managers.mn.sexMN.SexCountChange(fromTo.Value.To, fromTo.Value.From, SexManager.SexCountState.Toilet);
			// }
		}

		private void OnDelivery(object sender, SelfEventArgs e)
		{
			if (e.Self == null)
				return;

			if (!CommonUtils.IsPregnant(e.Self))
			{
				PLogger.LogWarning($"OnDelivery: Self '{e.Self.name}' is not pregnant");
				return;
			}

			// @TODO:
			// yield return new SpawnChild(scene, mother, delivery.SuccessRate).Handle();

			Managers.sexMN.SexCountChange(e.Self, null, SexManager.SexCountState.Delivery);
			e.Self.age++;

			Managers.sexMN.Pregnancy(e.Self, null, false);
		}

		private void OnMasturbation(object sender, SelfEventArgs e)
		{
			if (e.Self == null)
				return;

			e.Self.sexInfo[SexInfoIndex.Masturbation]++;
		}

		// @TODO:
		// private void OnPlayerDefeated(object sender, SelfEventArgs e)
		// {
		// 	Managers.mn.uiMN.MainCanvasView(false);
		// 	yield return Managers.mn.sexMN.StartCoroutine(Managers.mn.sound.GoBGMFade(1));
		// 	GameObject.Find("UIFXPool").transform.Find("ReviveSlider").GetComponent<Slider>().gameObject.SetActive(false);
		// 	yield return new WaitForSeconds(1f);
		// 	Managers.mn.uiMN.SkipView(true);
		// }

		private void OnSexEnd(object sender, SexEventArgs e)
		{
			if (e.ctx.SexScript is CommonSexNPCScript commonSexNpcScript)
			{
				if (commonSexNpcScript.treeState != Tree.Node.State.Success)
					return;

				// Sex was completed, every actor loves the others more.
				// Official code only works for 2 actors, but we are generalizing here so custom scripts may support more than 2.
				foreach (var actor in e.ctx.Actors)
				{
					foreach (var otherActor in e.ctx.Actors)
					{
						if (actor == otherActor)
							continue;

						actor.Common.LoveChange(otherActor.Common, 10f, false);
					}
				}
			}
			else if (e.ctx.SexScript is CommonSexPlayerScript commonSexPlayerScript)
			{
				if (!e.ctx.HasSexMeter)
					return;

				float loveChange = 0f;
				if (SexMeter.Instance.FillAmount == 1f)
					loveChange = 10f;
				else if (SexMeter.Instance.FillAmount < 0.3f)
					loveChange = -5f;

				if (loveChange != 0f) {
					var currentPlayer = CommonUtils.GetActivePlayer();
					foreach (var npcActor in e.ctx.Actors)
					{
						if (npcActor.Common == currentPlayer)
							continue;

						npcActor.Common.LoveChange(currentPlayer, loveChange, false);
					}
				}
			}

			// @TODO:
			// if (this.ToiletCounted.ContainsKey(scene))
			// 	this.ToiletCounted.Remove(scene);

			// if (ctx is PlayerRaped)
			// {
			// 	var playerRaped = scene as PlayerRaped;
			// 	if (playerRaped == null)
			// 		yield break;

			// 	if (playerRaped.Rapist.debuff.discontent == 4)
			// 		playerRaped.Rapist.MoralChange(20f, null, NPCManager.MoralCause.None);
			// }
		}
	}
}
