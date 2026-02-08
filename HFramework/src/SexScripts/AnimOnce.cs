#nullable enable

using UnityEngine;

namespace HFramework.SexScripts
{
	public class AnimOnce : BaseState
	{
		private string animName;

		private float animTime;

		private BaseState OnSuccessState;

		public AnimOnce(string animName, BaseState onSuccessState)
		{
			this.animName = animName;
			this.OnSuccessState = onSuccessState;
		}

		public override void OnEnter(CommonSexNpcScript ctx)
		{
			if (ctx.TmpSexAnim == null) {
				ctx.ShouldStop = true;
				return;
			}

			//@TODO: We need to handle the dynamic animation name (E.g. change for tit size)
			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			ctx.TmpSexAnim.state.SetAnimation(0, this.animName, true);
			this.animTime = ctx.TmpSexAnim.state.GetCurrent(0).AnimationEnd;
		}

		public override void Update(CommonSexNpcScript ctx)
		{
			//@TODO: Support skipping
			this.animTime -= Time.deltaTime;
			if (this.animTime <= 0) {
				ctx.ChangeState(this.OnSuccessState);
			}
		}

		public override void OnExit(CommonSexNpcScript ctx)
		{
			if (this.animTime > 0) {
				ctx.ShouldStop = true;
			}
		}
	}
}
