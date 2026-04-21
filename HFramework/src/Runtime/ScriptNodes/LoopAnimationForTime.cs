using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Animation/Loop Animation For Time")]
	public class LoopAnimationForTime : Action
	{
		public float Duration = 1;

		public string AnimationName = "";

		private TemplatedString TemplatedAnimationName;
		float remainingTime;

		private void Awake() {
			this.TemplatedAnimationName = new TemplatedString(this.AnimationName);
		}

		protected override void OnStart() {
			remainingTime = Duration;
			if (this.Context.TmpSexAnim == null) {
				PLogger.LogError("LoopAnimForTime: TmpSexAnim is null");
				return;
			}

			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			var animationName = this.TemplatedAnimationName.GetString(this.Context.Variables);
			if (!this.Context.TmpSexAnim.HasAnimation(animationName)) {
				PLogger.LogError($"LoopAnimForTime: Animation '{animationName}' not found");
				return;
			}

			this.Context.TmpSexAnim.state.SetAnimation(0, animationName, true);
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0) {
				return State.Success;
			}

			return State.Running;
		}
	}
}
