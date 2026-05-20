using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes.UI
{
	/// <summary>
	/// Toggles the visibility of the skip button UI element, which is shown on Player Raped interactions.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "UI/Toggle Skip Button")]
	[Obsolete("Skippable Section is now taking care of that.")]
	public class ToggleSkipButton : Action
	{
		[Tooltip("Expected visibility state of the skip button")]
		public bool ToVisibility;

		protected override void OnStart() {
			this.Context.HasChangedSkipButtonVisibility = true;
		}

		protected override State OnUpdate() {
			Managers.mn.uiMN.SkipView(ToVisibility);

			return State.Success;
		}

		protected override void OnStop() {
		}

		public void OnValidate() {
			Debug.LogError("ToggleSkipButton is obsolete and should not be used. Migrate to Skippable Section instead.");
		}
	}
}
