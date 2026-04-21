using YotanModCore;

namespace HFramework.ScriptNodes.Other
{
	[ScriptNode("HFramework", "Other/Toggle Game Control")]
	public class ToggleGameControl : Action
	{
		public bool ControlState;

		public bool Invincible;

		protected override void OnStart() {
		}

		protected override State OnUpdate() {
			Managers.mn.gameMN.Controlable(this.ControlState, this.Invincible);
			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
