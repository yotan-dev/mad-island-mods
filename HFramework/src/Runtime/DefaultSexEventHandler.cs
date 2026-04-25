using HFramework.Scenes;
using HFramework.SexScripts;
using HFramework.ScriptNodes;
using HFramework.Events;
using YotanModCore;
using YotanModCore.Consts;
using HFramework.SexScripts.ScriptContext;

namespace HFramework
{
	internal class DefaultSexEventHandler
	{
		internal static DefaultSexEventHandler Instance { get; private set; } = new DefaultSexEventHandler();

		public void Init() {
			SexEvents.OnPerformMasturbation.Triggered += this.OnMasturbation;
			SexEvents.OnPerformDelivery.Triggered += this.OnDelivery;

			SexEvents.OnPenetrateVagina.Triggered += this.OnPenetrationStart;
			SexEvents.OnPenetrateAss.Triggered += this.OnPenetrationStart;

			SexEvents.OnCumOnVagina.Triggered += this.OnCumOnVagina;

			SexEvents.OnEnd.Triggered += this.OnSexEnd_CommonSexNpc;
			SexEvents.OnEnd.Triggered += this.OnSexEnd_CommonSexPlayer;
			SexEvents.OnEnd.Triggered += this.OnSexEnd_PlayerRaped;
		}

		private void OnCumOnVagina(object sender, FromToEventArgs e) {
			Managers.mn.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Creampie);
		}

		private void OnPenetrationStart(object sender, FromToEventArgs e) {
			if (e.ctx.SexScript.Info.ContextTags.Contains(ContextTags.Normal)) {
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Normal);
			}
		}

		private void OnDelivery(object sender, SelfEventArgs e) {
			if (e.Self == null)
				return;

			if (!CommonUtils.IsPregnant(e.Self)) {
				PLogger.LogWarning($"OnDelivery: Self '{e.Self.name}' is not pregnant");
				return;
			}

			// @TODO:
			// yield return new SpawnChild(scene, mother, delivery.SuccessRate).Handle();

			Managers.sexMN.SexCountChange(e.Self, null, SexManager.SexCountState.Delivery);
			e.Self.age++;

			Managers.sexMN.Pregnancy(e.Self, null, false);
		}

		private void OnMasturbation(object sender, SelfEventArgs e) {
			if (e.Self == null)
				return;

			e.Self.sexInfo[SexInfoIndex.Masturbation]++;
		}

		private void OnSexEnd_CommonSexPlayer(object sender, SexEventArgs e) {
			if (e.ctx.SexScript is not CommonSexPlayerScript commonSexPlayerScript)
				return;

			if (!e.ctx.HasSexMeter)
				return;

			float loveChange = 0f;
			if (SexMeter.Instance.FillAmount == 1f)
				loveChange = 10f;
			else if (SexMeter.Instance.FillAmount < 0.3f)
				loveChange = -5f;

			if (loveChange != 0f) {
				var currentPlayer = CommonUtils.GetActivePlayer();
				foreach (var npcActor in e.ctx.Actors) {
					if (npcActor.Common == currentPlayer)
						continue;

					npcActor.Common.LoveChange(currentPlayer, loveChange, false);
				}
			}
		}

		private void OnSexEnd_PlayerRaped(object sender, SexEventArgs e) {
			if (e.ctx.SexScript is not PlayerRapedScript playerRapedScript)
				return;

			// Success means the rape encounter went until the end (player didn't escape)
			if (e.ctx.MainNodeState != ScriptNode.State.Success)
				return;

			var rapist = e.ctx.Actors[0]?.Common ?? null;
			var victim = e.ctx.Actors[1]?.Common ?? null;
			if (rapist == null || victim == null) {
				PLogger.LogWarning($"OnSexEnd_PlayerRaped: Rapist or victim is null");
				return;
			}

			if (victim != CommonUtils.GetActivePlayer()) {
				PLogger.LogWarning($"OnSexEnd_PlayerRaped: Victim is not the active player");
				return;
			}

			if (rapist.debuff.discontent == 4)
				rapist.MoralChange(20f);

			victim.life = (int)(victim.maxLife * 0.10);
			victim.CommonLifeChange(0.0);
			victim.faint = (int)(victim.maxFaint * 0.20);
			Managers.mn.gameMN.FaintImageChange();
			Managers.sexMN.StartCoroutine(Managers.sexMN.ReviveToNearPoint(rapist.npcID));
		}

		private void OnSexEnd_CommonSexNpc(object sender, SexEventArgs e) {
			if (e.ctx.SexScript is not CommonSexNPCScript commonSexNpcScript)
				return;

			if (commonSexNpcScript.TreeState != ScriptNode.State.Success)
				return;

			// Sex was completed, every actor loves the others more.
			// Official code only works for 2 actors, but we are generalizing here so custom scripts may support more than 2.
			foreach (var actor in e.ctx.Actors) {
				foreach (var otherActor in e.ctx.Actors) {
					if (actor == otherActor)
						continue;

					actor.Common.LoveChange(otherActor.Common, 10f, false);
				}
			}
		}
	}
}
