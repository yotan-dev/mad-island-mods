using HFramework.Scenes;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Sex Meter/Toggle Sex Meter")]
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
