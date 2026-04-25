using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Other/Toggle Man View")]
	public class ToggleManView : Action
	{
		public int TrackIndex = 10;
		public string AnimationName = "noman";

		private TemplatedString templatedAnimName;

		private void Awake() {
			this.templatedAnimName = new TemplatedString(this.AnimationName);
		}

		protected override void OnStart() {

		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			if (this.Context.TmpSexAnim == null) {
				// While this is an issue, we can keep the system running -- probably
				PLogger.LogError("ToggleManView: TmpSexAnim is null");
				return State.Success;
			}

			var animationName = this.templatedAnimName.GetString(this.Context.Variables);
			if (!this.Context.TmpSexAnim.HasAnimation(animationName)) {
				// While this is an issue, we can keep the system running -- probably
				PLogger.LogError($"ToggleManView: Animation '{animationName}' not found");
				return State.Success;
			}

			if (this.Context.TmpSexAnim.state.GetCurrent(this.TrackIndex) != null) {
				this.Context.TmpSexAnim.state.SetEmptyAnimation(this.TrackIndex, 0);
			} else {
				this.Context.TmpSexAnim.state.SetAnimation(this.TrackIndex, animationName, true);
			}

			return State.Success;
		}
	}
}
