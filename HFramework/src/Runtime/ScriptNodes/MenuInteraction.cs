#nullable enable

namespace HFramework.ScriptNodes
{
	/// <summary>
	/// Node to handle menu interactions. All of its children must be LastChoiceEquals nodes.
	///
	/// This node will look for Context.PendingChoiceId and Context.PendingChoiceAction and trigger the corresponding children nodes.
	///
	/// PendingChoiceAction is used for nodes that are quickly fired and succeed. They are executed immediately and parallel to the main menu state.
	///
	/// PendingChoiceId is used for long running paths. When a new one is set, the current child (if any) is forcefully terminated,
	/// and the new one is then started.
	/// Until the children finishes (succeeds/fails), it will continue to be updated on every tick.
	/// Once the children finishes, MenuInteraction will keep running without calling any children, until a new choice is given.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Menu/Menu Interaction")]
	public class MenuInteraction : Composite
	{
		public string InitialMenuId = "";

		public string SuccessOnChoiceId = "leave";

		private ScriptNode? CurrentChild;

		protected override void OnStart() {
			this.Context.PendingChoiceId = this.InitialMenuId;
		}

		protected override void OnStop() {

		}

		private bool HandleAction() {
			var actionNode = this.Children.Find(node => (node as LastChoiceEquals)?.ChoiceId == this.Context.PendingChoiceAction);
			if (actionNode != null) {
				var actionResult = actionNode.Update();
				switch (actionResult) {
					case State.Running:
						PLogger.LogWarning($"MenuInteractionNode: Unexpected ACTION menu '{this.Context.PendingChoiceAction}' moving to Running state -- they should be a single fire operation");
						break;

					case State.Failure:
						PLogger.LogWarning($"MenuInteractionNode: ACTION menu '{this.Context.PendingChoiceAction}' failed");
						return false; // Abort everything
				}
			} else {
				PLogger.LogWarning($"MenuInteractionNode: ACTION menu '{this.Context.PendingChoiceAction}' not found");
			}

			this.Context.PendingChoiceAction = null;
			return true;
		}

		protected override State OnUpdate() {
			PLogger.LogDebug("MenuInteractionNode: " + this.Context.PendingChoiceId);
			if (this.Context.PendingChoiceId == this.SuccessOnChoiceId) {
				PLogger.LogDebug("MenuInteractionNode: Success");
				return State.Success;
			}

			if (this.Context.PendingChoiceAction != null) {
				if (!HandleAction()) {
					return State.Failure;
				}
			}

			// Consume pending choice
			if (this.Context.PendingChoiceId != null) {
				var targetChild = this.Children.Find((node) => {
					if (node is LastChoiceEquals choice) {
						return choice.ChoiceId == this.Context.PendingChoiceId;
					}

					PLogger.LogError("MenuInteractionNode: " + node.name + " is not a LastChoiceEqualsNode");
					return false;
				});

				if (targetChild == null) {
					PLogger.LogWarning("MenuInteractionNode: " + this.Context.PendingChoiceId + " not found");
					return State.Failure;
				}

				this.CurrentChild?.Terminate();
				this.CurrentChild = targetChild;

				this.Context.PendingChoiceId = null;
			}

			// If there is no child to run, we just wait, probably the last choice is already consumed and just need to stay as is
			if (this.CurrentChild == null) {
				PLogger.LogDebug("MenuInteractionNode: No pending node");
				return State.Running;
			}

			// There is a child to run, run it now.
			switch (this.CurrentChild.Update()) {
				case State.Running:
					return State.Running;

				case State.Success:
					this.CurrentChild = null;
					return State.Running;

				case State.Failure:
					this.CurrentChild = null;
					return State.Failure;

				default:
					PLogger.LogWarning($"MenuInteractionNode: Unexpected state");
					return State.Failure;
			}
		}
	}
}
