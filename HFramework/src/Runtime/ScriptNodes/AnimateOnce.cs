using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Animation/Animate Once")]
	public class AnimateOnce : Action
	{
		public string AnimationName = "";

		private TemplatedString TemplatedAnimationName;
		private float RemainingTime;

		private void Awake() {
			this.TemplatedAnimationName = new TemplatedString(this.AnimationName);
		}

		protected override void OnStart() {
			if (this.Context.TmpSexAnim == null) {
				PLogger.LogError("AnimOnce: TmpSexAnim is null");
				return;
			}

			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			var animationName = this.TemplatedAnimationName.GetString(this.Context.Variables);
			if (!this.Context.TmpSexAnim.HasAnimation(animationName)) {
				PLogger.LogError($"AnimOnce: Animation '{animationName}' not found");
				return;
			}

			this.Context.TmpSexAnim.state.SetAnimation(0, animationName, false);
			this.RemainingTime = this.Context.TmpSexAnim.state.GetCurrent(0).AnimationEnd;
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			this.RemainingTime -= Time.deltaTime;
			if (this.RemainingTime <= 0) {
				return State.Success;
			}

			return State.Running;
		}
	}
}
