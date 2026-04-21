using YotanModCore;

namespace HFramework.ScriptNodes.UI
{
	/// <summary>
	/// Toggles the visibility of the skip button UI element, which is shown on Player Raped interactions.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "UI/Toggle Skip Button")]
	public class ToggleSkipButton : Action
	{
		public bool ToVisibility;

		protected override void OnStart() {
			this.context.HasChangedSkipButtonVisibility = true;
		}

		protected override State OnUpdate() {
			Managers.mn.uiMN.SkipView(ToVisibility);

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
