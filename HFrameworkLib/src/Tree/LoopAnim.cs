using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Tree
{
	public class LoopAnim : ActionNode
	{
		public string animName = "";

		float remainingTime;

		protected override void OnStart()
		{
			if (this.context.TmpSexAnim == null)
			{
				PLogger.LogError("LoopAnimForTime: TmpSexAnim is null");
				return;
			}

			//@TODO: We need to handle the dynamic animation name (E.g. change for tit size)
			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			if (!this.context.TmpSexAnim.HasAnimation(this.animName))
			{
				PLogger.LogError($"LoopAnimForTime: Animation '{this.animName}' not found");
				return;
			}

			this.context.TmpSexAnim.state.SetAnimation(0, this.animName, true);
		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			return State.Running;
		}
	}
}
