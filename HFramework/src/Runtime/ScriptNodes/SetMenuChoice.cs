namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Menu/Set Menu Choice")]
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
