using YotanModCore;

namespace HFramework.ScriptNodes.Characters
{
	[ScriptNode("HFramework", "Characters/Toggle Player Visible")]
	public class TogglePlayerVisible : Action
	{
		public bool ToVisibility;

		protected override void OnStart() {
		}

		protected override State OnUpdate() {
			Managers.mn.gameMN.pMove.PlayerVisible(this.ToVisibility);
			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
