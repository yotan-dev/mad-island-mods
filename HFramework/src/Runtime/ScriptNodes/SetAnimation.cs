using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	/// <summary>
	/// Sets the animation for the current sex scene in loop and succeed
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Animation/Set Animation")]
	public class SetAnimation : Action
	{
		public string AnimationName = "";

		private TemplatedString TemplatedAnimationName;

		private string FinalAnimationName;

		private void Awake() {
			this.TemplatedAnimationName = new TemplatedString(this.AnimationName);
		}

		protected override void OnStart() {
			if (this.Context.TmpSexAnim == null) {
				PLogger.LogError("SetAnimForTime: TmpSexAnim is null");
				return;
			}

			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			var animationName = this.TemplatedAnimationName.GetString(this.Context.Variables);
			if (!this.Context.TmpSexAnim.HasAnimation(animationName)) {
				PLogger.LogError($"SetAnimForTime: Animation '{animationName}' not found");
				return;
			}

			this.FinalAnimationName = animationName;
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			this.Context.TmpSexAnim.state.SetAnimation(0, this.FinalAnimationName, true);
			return State.Success;
		}
	}
}
