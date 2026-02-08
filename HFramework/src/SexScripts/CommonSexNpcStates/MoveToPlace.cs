using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;

namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class MoveToPlace : BaseState
	{
		private readonly BaseState NextState;

		private float animTime;

		private GameObject[] emotions;

		public MoveToPlace(BaseState onPositionReachedState)
		{
			NextState = onPositionReachedState;
		}

		public override void OnEnter(CommonSexNpcScript ctx)
		{
			this.animTime = 30f;

			// Start moving NPCs to sex place
			this.emotions = new GameObject[2];
			this.emotions[0] = Managers.fxMN.GoEmotion(0, ctx.NpcA.gameObject, Vector3.zero);
			this.emotions[1] = Managers.fxMN.GoEmotion(0, ctx.NpcB.gameObject, Vector3.zero);
			Managers.sexMN.StartCoroutine(
				Managers.storyMN.MovePosition(ctx.NpcA.gameObject, ctx.SexPlacePos.Value, 2f, AnimName.Walk, true)
			);
			Managers.sexMN.StartCoroutine(
				Managers.storyMN.MovePosition(ctx.NpcB.gameObject, ctx.SexPlacePos.Value, 2f, AnimName.Walk, true)
			);
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

		public override void Update(CommonSexNpcScript ctx)
		{
			this.animTime -= Time.deltaTime;

			// If NPCs reached target position, we are done.
			var isNpcAAtPos = this.IsNpcAtPos(ctx.NpcA, ctx.SexPlacePos.Value);
			var isNpcBAtPos = this.IsNpcAtPos(ctx.NpcB, ctx.SexPlacePos.Value);
			if (isNpcAAtPos && isNpcBAtPos)
			{
				ctx.ChangeState(this.NextState);
				return;
			}

			// Otherwise, we need to check for possible interruptions.

			if (!this.IsNpcWaiting(ctx.NpcA) || !this.IsNpcWaiting(ctx.NpcB))
			{ // Something else took the "attention" of one of the NPCs -- give up
				ctx.ShouldStop = true;
				return;
			}

			if (ctx.SexPlace.user != null)
			{ // Some other NPC took the sex place, can't use it -- give up
				ctx.ShouldStop = true;
				return;
			}

			if (this.animTime <= 0f)
			{ // Timeout reached -- give up
				ctx.ShouldStop = true;
				return;
			}
		}

		public override void OnExit(CommonSexNpcScript ctx)
		{
			if (this.emotions == null)
				return;

			foreach (var emotion in this.emotions)
			{
				emotion?.SetActive(false);
			}
		}
	}
}
