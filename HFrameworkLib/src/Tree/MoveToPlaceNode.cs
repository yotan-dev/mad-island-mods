using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;

namespace HFramework.Tree
{
	public class MoveToPlaceNode : ActionNode
	{
		public float timeout = 30f;

		private float animTime;

		private GameObject[] emotions;

		protected override void OnStart()
		{
			this.animTime = this.timeout;

			// Start moving NPCs to sex place
			this.emotions = new GameObject[this.context.Npcs.Length];
			for (int i = 0; i < this.context.Npcs.Length; i++)
			{
				this.emotions[i] = Managers.fxMN.GoEmotion(0, this.context.Npcs[i].Common.gameObject, Vector3.zero);
			}
			for (int i = 0; i < this.context.Npcs.Length; i++)
			{
				Managers.sexMN.StartCoroutine(
					Managers.storyMN.MovePosition(this.context.Npcs[i].Common.gameObject, this.context.SexPlacePos.Value, 2f, AnimName.Walk, true)
				);
			}
		}

		private bool IsNpcAtPos(CommonStates npc, Vector3 pos)
		{
			// Delivery checks for 0.5 instead of 1, but 1 for everything should be good enough.
			if (Vector3.Distance(npc.gameObject.transform.position, pos) > 1f)
			{
				if (npc.anim.GetCurrentAnimName() != AnimName.Walk && npc.nMove.actType != NPCMove.ActType.Interval)
					npc.anim.state.SetAnimation(0, AnimName.Walk, true);

				return false;
			}

			if (npc.nMove.common.anim.GetCurrentAnimName() != AnimName.Idle)
				npc.nMove.common.anim.state.SetAnimation(0, AnimName.Idle, true);

			return true;
		}

		private bool IsNpcWaiting(CommonStates npc)
		{
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

		protected override void OnStop()
		{
			if (this.emotions == null)
				return;

			foreach (var emotion in this.emotions)
			{
				emotion?.SetActive(false);
			}
		}

		protected override State OnUpdate()
		{
			this.animTime -= Time.deltaTime;

			// If NPCs reached target position, we are done.
			bool allAtPos = true;
			for (int i = 0; i < this.context.Npcs.Length; i++)
			{
				if (!this.IsNpcAtPos(this.context.Npcs[i].Common, this.context.SexPlacePos.Value))
				{
					allAtPos = false;
					break;
				}
			}

			if (allAtPos)
			{
				return State.Success;
			}

			// Otherwise, we need to check for possible interruptions.

			bool allWaiting = true;
			for (int i = 0; i < this.context.Npcs.Length; i++)
			{
				if (!this.IsNpcWaiting(this.context.Npcs[i].Common))
				{
					allWaiting = false;
					break;
				}
			}
			if (!allWaiting)
			{ // Something else took the "attention" of one of the NPCs -- give up
				return State.Failure;
			}

			if (this.context.SexPlace.user != null)
			{ // Some other NPC took the sex place, can't use it -- give up
				return State.Failure;
			}

			if (this.animTime <= 0f)
			{ // Timeout reached -- give up
				return State.Failure;
			}

			return State.Running;
		}
	}
}
