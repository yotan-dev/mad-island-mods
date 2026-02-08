#nullable enable

using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.SexScripts
{
	public class LoopAnimForTime : BaseState
	{
		private string animName;

		private float time;

		private float animTime;

		private BaseState OnSuccessState;

		public LoopAnimForTime(string animName, float time, BaseState onSuccessState)
		{
			this.animName = animName;
			this.time = time;
			this.OnSuccessState = onSuccessState;
		}

		public override void OnEnter(CommonSexNpcScript ctx)
		{
			if (ctx.TmpSexAnim == null) {
				PLogger.LogError("LoopAnimForTime: TmpSexAnim is null");
				ctx.ShouldStop = true;
				return;
			}

			//@TODO: We need to handle the dynamic animation name (E.g. change for tit size)
			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			this.animTime = this.time;

			if (!ctx.TmpSexAnim.HasAnimation(this.animName)) {
				PLogger.LogError($"LoopAnimForTime: Animation '{this.animName}' not found");
				return;
			}

			ctx.TmpSexAnim.state.SetAnimation(0, this.animName, true);
		}

		public override void Update(CommonSexNpcScript ctx)
		{
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
