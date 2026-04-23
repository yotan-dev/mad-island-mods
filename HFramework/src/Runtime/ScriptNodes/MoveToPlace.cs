using UnityEngine;
using YotanModCore;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Characters/Move To Place")]
	public class MoveToPlace : Action
	{
		public float TimeLimitSeconds = 30f;

		private float AnimationTime;

		private GameObject[] Emotions;

		protected override void OnStart() {
			this.AnimationTime = this.TimeLimitSeconds;
			var targetPos = this.Context.ScriptPlace.GetCharacterPosition();

			// Start moving NPCs to sex place
			this.Emotions = new GameObject[this.Context.Actors.Length];
			for (int i = 0; i < this.Context.Actors.Length; i++) {
				this.Emotions[i] = Managers.fxMN.GoEmotion(0, this.Context.Actors[i].Common.gameObject, Vector3.zero);
			}
			for (int i = 0; i < this.Context.Actors.Length; i++) {
				Managers.sexMN.StartCoroutine(
					Managers.storyMN.MovePosition(this.Context.Actors[i].Common.gameObject, targetPos, 2f, "A_walk", true)
				);
			}
		}

		private bool IsNpcAtPos(CommonStates npc, Vector3 pos) {
			// Delivery checks for 0.5 instead of 1, but 1 for everything should be good enough.
			if (Vector3.Distance(npc.gameObject.transform.position, pos) > 1f) {
				if (npc.anim.GetCurrentAnimName() != "A_walk" && npc.nMove.actType != NPCMove.ActType.Interval)
					npc.anim.state.SetAnimation(0, "A_walk", true);

				return false;
			}

			if (npc.nMove.common.anim.GetCurrentAnimName() != "A_idle")
				npc.nMove.common.anim.state.SetAnimation(0, "A_idle", true);

			return true;
		}

		private bool IsNpcWaiting(CommonStates npc) {
			var player = CommonUtils.GetActivePlayer();
			if (npc == player)
				return true;

			// Official code only checks for Interval in delivery, but it looks like
			// Interval is set in MovePosition , so it would be safer to check here too..
			return (
				npc.nMove.actType == NPCMove.ActType.Wait
				|| npc.nMove.actType == NPCMove.ActType.Interval
			);
		}

		protected override void OnStop() {
			if (this.Emotions == null)
				return;

			foreach (var emotion in this.Emotions) {
				emotion?.SetActive(false);
			}
		}

		protected override State OnUpdate() {
			this.AnimationTime -= Time.deltaTime;
			var targetPos = this.Context.ScriptPlace.GetCharacterPosition();

			// If NPCs reached target position, we are done.
			bool allAtPos = true;
			for (int i = 0; i < this.Context.Actors.Length; i++) {
				if (!this.IsNpcAtPos(this.Context.Actors[i].Common, targetPos)) {
					allAtPos = false;
					break;
				}
			}

			if (allAtPos) {
				return State.Success;
			}

			// Otherwise, we need to check for possible interruptions.

			bool allWaiting = true;
			for (int i = 0; i < this.Context.Actors.Length; i++) {
				if (!this.IsNpcWaiting(this.Context.Actors[i].Common)) {
					allWaiting = false;
					break;
				}
			}
			if (!allWaiting) { // Something else took the "attention" of one of the NPCs -- give up
				return State.Failure;
			}

			if (this.Context.ScriptPlace.IsInUse()) { // Some other NPC took the sex place, can't use it -- give up
				return State.Failure;
			}

			if (this.AnimationTime <= 0f) { // Timeout reached -- give up
				return State.Failure;
			}

			return State.Running;
		}
	}
}
