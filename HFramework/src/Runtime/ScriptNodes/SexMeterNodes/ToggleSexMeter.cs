using HFramework.Scenes;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.SexMeterNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Sex Meter/Toggle Sex Meter")]
	[MovedFrom(true, "HFramework.ScriptNodes", null, "ToggleSexMeter")]
	public class ToggleSexMeter : Action
	{
		public bool ToVisibility = true;

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			if (this.ToVisibility) {
				SexMeter.Instance.Show();
			} else {
				SexMeter.Instance.Hide();
			}

			return State.Success;
		}
	}
}
