namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Menu/Last Choice Equals")]
	public class LastChoiceEquals : Passthrough
	{
		public string ChoiceId = "";

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			if (string.IsNullOrEmpty(this.ChoiceId)) {
				PLogger.LogError("LastChoiceEqualsNode: choiceId is empty");
				return State.Failure;
			}

			if (this.Context.PendingChoiceId != this.ChoiceId && this.Context.PendingChoiceAction != this.ChoiceId) {
				PLogger.LogError($"LastChoiceEqualsNode: Triggered a Choice Node that was not expected. Expected choice '{this.ChoiceId}' but got '{this.Context.PendingChoiceId}' or action '{this.Context.PendingChoiceAction}'");
				return State.Failure;
			}

			return this.Child != null ? this.Child.Update() : State.Success;
		}
	}
}
