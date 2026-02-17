using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Tree
{
	public class LoopAnimForTimeNode : ActionNode
	{
		public float duration = 1;

		public string animName = "";

		private TemplatedString templatedAnimName;
		float remainingTime;

		private void Awake()
		{
			this.templatedAnimName = new TemplatedString(this.animName);
		}

		protected override void OnStart()
		{
			remainingTime = duration;
			if (this.context.TmpSexAnim == null)
			{
				PLogger.LogError("LoopAnimForTime: TmpSexAnim is null");
				return;
			}

			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			var animationName = this.templatedAnimName.GetString(this.context.Variables);
			if (!this.context.TmpSexAnim.HasAnimation(animationName))
			{
				PLogger.LogError($"LoopAnimForTime: Animation '{animationName}' not found");
				return;
			}

			this.context.TmpSexAnim.state.SetAnimation(0, animationName, true);
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
