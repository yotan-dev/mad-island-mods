using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Tree
{
	public class AnimOnceNode : ActionNode
	{
		public string animName = "";

		float remainingTime;

		protected override void OnStart()
		{
			if (this.context.TmpSexAnim == null)
			{
				PLogger.LogError("AnimOnce: TmpSexAnim is null");
				return;
			}

			//@TODO: We need to handle the dynamic animation name (E.g. change for tit size)
			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			if (!this.context.TmpSexAnim.HasAnimation(this.animName))
			{
				PLogger.LogError($"AnimOnce: Animation '{this.animName}' not found");
				return;
			}

			this.context.TmpSexAnim.state.SetAnimation(0, this.animName, true);
			this.remainingTime = this.context.TmpSexAnim.state.GetCurrent(0).AnimationEnd;
		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0)
			{
				return State.Success;
			}

			return State.Running;
		}
	}
}
