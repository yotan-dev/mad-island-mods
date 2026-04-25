using HFramework.SexScripts.ScriptContext;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Other/Set Sex Action")]
	public class SetSexAction : Action
	{
		public SexAction Action;

		protected override void OnStart() {

		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			this.Context.SexAction = this.Action;
			return State.Success;
		}
	}
}
