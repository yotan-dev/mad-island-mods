using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Animation/Loop Animation")]
	public class LoopAnimation : Action
	{
		public string AnimationName = "";

		private TemplatedString TemplatedAnimationName;

		private void Awake() {
			this.TemplatedAnimationName = new TemplatedString(this.AnimationName);
		}

		protected override void OnStart() {
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
			return State.Running;
		}
	}
}
