using UnityEngine;
using UnityEngine.UI;

namespace HFramework.ScriptNodes.UI
{
	/// <summary>
	/// Toggles the visibility of the revive slider UI element.
	/// This is the "Click" input when the character is downed and struggling to get back up.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "UI/Toggle Revive Slider")]
	public class ToggleReviveSlider : Action
	{
		[Tooltip("Expected visibility state of the revive slider")]
		public bool ToVisibility;

		protected override void OnStart() {
		}

		protected override State OnUpdate() {
			GameObject.Find("UIFXPool").transform.Find("ReviveSlider").GetComponent<Slider>().gameObject.SetActive(ToVisibility);

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
