using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Menu
{
	[Experimental]
	[ScriptNode("HFramework", "Menu/Set Menu Choice")]
	[MovedFrom(true, "HFramework.ScriptNodes", null, "SetMenuChoice")]
	public class SetMenuChoice : Action
	{
		public string NewChoiceId;

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			this.Context.PendingChoiceId = this.NewChoiceId;
			return State.Success;
		}
	}
}
