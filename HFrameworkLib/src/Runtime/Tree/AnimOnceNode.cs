using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Tree
{
	public class AnimOnceNode : ActionNode
	{
		public string animName = "";

		private TemplatedString templatedAnimName;
		float remainingTime;

		private void Awake()
		{
			this.templatedAnimName = new TemplatedString(this.animName);
		}

		protected override void OnStart()
		{
			if (this.context.TmpSexAnim == null)
			{
				PLogger.LogError("AnimOnce: TmpSexAnim is null");
				return;
			}

			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			var animationName = this.templatedAnimName.GetString(this.context.Variables);
			if (!this.context.TmpSexAnim.HasAnimation(animationName))
			{
				PLogger.LogError($"AnimOnce: Animation '{animationName}' not found");
				return;
			}

			this.context.TmpSexAnim.state.SetAnimation(0, animationName, false);
			this.remainingTime = this.context.TmpSexAnim.state.GetCurrent(0).AnimationEnd;
		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			this.remainingTime -= Time.deltaTime;
			if (this.remainingTime <= 0)
			{
				return State.Success;
			}

			return State.Running;
		}
	}
}
