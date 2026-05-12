using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Characters/Move To Place")]
	public class MoveToPlace : Action
	{
		[Tooltip("Maximum time to wait for NPCs to reach the target position.")]
		public float TimeLimitSeconds = 30f;

		[Tooltip("Whether NPC should try to avoid obstacles when moving. For some reason this does not work when 2+ NPCs are moving at the same time.")]
		public bool AvoidObstacles = false;

		[Tooltip("Emote to play while moving.")]
		[EmoteId]
		public int EmoteOnMove = Emote.None;

		private bool IsReady = false;

		private float AnimationTime;

		private GameObject[] Emotions;

		protected override void OnStart() {
			this.AnimationTime = this.TimeLimitSeconds;
			this.IsReady = false;
			var targetPos = this.Context.ScriptPlace.GetCharacterPosition();

			// Start moving NPCs to sex place
			this.Emotions = null;
			if (this.EmoteOnMove != Emote.None) {
				this.Emotions = new GameObject[this.Context.Actors.Length];
				for (int i = 0; i < this.Context.Actors.Length; i++) {
					this.Emotions[i] = Managers.fxMN.GoEmotion(this.EmoteOnMove, this.Context.Actors[i].Common.gameObject, Vector3.zero);
				}
			}

			for (int i = 0; i < this.Context.Actors.Length; i++) {
				Managers.sexMN.StartCoroutine(
					Managers.storyMN.MovePosition(this.Context.Actors[i].Common.gameObject, targetPos, 2f, "A_walk", true, this.AvoidObstacles)
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
			if (this.Context.ScriptPlace.IsInUse()) { // Some other NPC took the sex place, can't use it -- give up
				return State.Failure;
			}

			if (this.IsReady) {
				return State.Success;
			}

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
				// Force them into the real position so that MovePosition can properly finish and return.
				// We give a tick for this to happen and complete on the next tick.
				//
				// That's because MovePosition actually tries to hit 0.1 distance before completing,
				// while MoveToPlacae ends at 0.5 distance. It is messy and we can't just use MovePosition,
				// because MovePosition loses the animation data and also can't seem to be able to get in place for some reason
				//
				// Probably it would be better to just have our own version of MovePosition, but too much work for now.
				foreach (var actor in this.Context.Actors) {
					actor.Common.transform.position = targetPos;
				}
				this.IsReady = true;
				return State.Running;
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

			if (this.AnimationTime <= 0f) { // Timeout reached -- give up
				return State.Failure;
			}

			return State.Running;
		}
	}
}
