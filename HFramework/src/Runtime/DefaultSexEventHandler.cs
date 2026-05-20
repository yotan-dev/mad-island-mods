using HFramework.Scenes;
using HFramework.SexScripts;
using HFramework.ScriptNodes;
using HFramework.Events;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework
{
	internal class DefaultSexEventHandler
	{
		internal static DefaultSexEventHandler Instance { get; private set; } = new DefaultSexEventHandler();

		public void Init() {
#pragma warning disable CS0618 // Type or member is obsolete
			SexEvents.OnPerformMasturbation.Triggered += this.OnMasturbation;
#pragma warning restore CS0618 // Type or member is obsolete
			SexEvents.OnSelfHand2Vagina.Triggered += this.OnMasturbation;
			SexEvents.OnSelfHand2Penis.Triggered += this.OnMasturbation;

			SexEvents.OnCumOnVagina.Triggered += this.OnCumOnVagina;

			SexEvents.OnEnd.Triggered += this.OnSexEnd_CommonSexNpc;
			SexEvents.OnEnd.Triggered += this.OnSexEnd_CommonSexPlayer;
			SexEvents.OnEnd.Triggered += this.OnSexEnd_PlayerRaped;
		}

		private void OnCumOnVagina(object sender, FromToEventArgs e) {
			Managers.mn.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Creampie);
		}

		private void OnMasturbation(object sender, SelfEventArgs e) {
			if (e.Self == null)
				return;

			e.Self.sexInfo[SexInfoIndex.Masturbation]++;
		}

		private void OnSexEnd_CommonSexPlayer(object sender, SexEventArgs e) {
			if (e.Ctx.SexScript is not CommonSexPlayerScript commonSexPlayerScript)
				return;

			if (!e.Ctx.HasSexMeter)
				return;

			float loveChange = 0f;
			if (SexMeter.Instance.FillAmount == 1f)
				loveChange = 10f;
			else if (SexMeter.Instance.FillAmount < 0.3f)
				loveChange = -5f;

			if (loveChange != 0f) {
				var currentPlayer = CommonUtils.GetActivePlayer();
				foreach (var npcActor in e.Ctx.Actors) {
					if (npcActor.Common == currentPlayer)
						continue;

					npcActor.Common.LoveChange(currentPlayer, loveChange, false);
				}
			}
		}

		private void OnSexEnd_PlayerRaped(object sender, SexEventArgs e) {
			if (e.Ctx.SexScript is not PlayerRapedScript playerRapedScript)
				return;

			// Success means the rape encounter went until the end (player didn't escape)
			if (e.Ctx.MainNodeState != ScriptNode.State.Success)
				return;

			var rapist = e.Ctx.Actors[0]?.Common ?? null;
			var victim = e.Ctx.Actors[1]?.Common ?? null;
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
			if (e.Ctx.SexScript is not CommonSexNPCScript commonSexNpcScript)
				return;

			if (commonSexNpcScript.TreeState != ScriptNode.State.Success)
				return;

			// Sex was completed, every actor loves the others more.
			// Official code only works for 2 actors, but we are generalizing here so custom scripts may support more than 2.
			foreach (var actor in e.Ctx.Actors) {
				foreach (var otherActor in e.Ctx.Actors) {
					if (actor == otherActor)
						continue;

					actor.Common.LoveChange(otherActor.Common, 10f, false);
				}
			}
		}
	}
}
