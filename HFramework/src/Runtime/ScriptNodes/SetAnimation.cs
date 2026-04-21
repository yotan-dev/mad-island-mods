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
		public string animName = "";

		private TemplatedString templatedAnimName;

		private string finalAnimName;

		private void Awake() {
			this.templatedAnimName = new TemplatedString(this.animName);
		}

		protected override void OnStart() {
			if (this.context.TmpSexAnim == null) {
				PLogger.LogError("SetAnimForTime: TmpSexAnim is null");
				return;
			}

			//@TODO: We may consider pausing the animation here and resuming later (see ResumeAnimation in DefaultSceneController)

			var animationName = this.templatedAnimName.GetString(this.context.Variables);
			if (!this.context.TmpSexAnim.HasAnimation(animationName)) {
				PLogger.LogError($"SetAnimForTime: Animation '{animationName}' not found");
				return;
			}

			this.finalAnimName = animationName;
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			this.context.TmpSexAnim.state.SetAnimation(0, this.finalAnimName, true);
			return State.Success;
		}
	}
}
