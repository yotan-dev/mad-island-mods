#nullable enable

namespace HFramework.ScriptNodes
{
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

		protected override State OnUpdate() {
			PLogger.LogDebug("MenuInteractionNode: " + this.Context.PendingChoiceId);
			if (this.Context.PendingChoiceId == this.SuccessOnChoiceId) {
				PLogger.LogDebug("MenuInteractionNode: Success");
				return State.Success;
			}

			if (this.Context.PendingChoiceAction != null) {
				var actionNode = this.Children.Find(node => (node as LastChoiceEquals)?.ChoiceId == this.Context.PendingChoiceAction);
				if (actionNode != null) {
					var actionResult = actionNode.Update();
					switch (actionResult) {
						case State.Running:
							PLogger.LogWarning($"MenuInteractionNode: Unexpected ACTION menu '{this.Context.PendingChoiceAction}' moving to Running state -- they should be a single fire operation");
							break;

						case State.Failure:
							PLogger.LogWarning($"MenuInteractionNode: ACTION menu '{this.Context.PendingChoiceAction}' failed");
							return State.Failure; // Abort everything
					}
				} else {
					PLogger.LogWarning($"MenuInteractionNode: ACTION menu '{this.Context.PendingChoiceAction}' not found");
				}

				this.Context.PendingChoiceAction = null;
			}

			if (this.Context.PendingChoiceId == null) {
				PLogger.LogDebug("MenuInteractionNode: No pending choice");
				return State.Running;
			}

			var targetChild = this.Children.Find((node) => {
				if (node is LastChoiceEquals choice) {
					return choice.ChoiceId == this.Context.PendingChoiceId;
				}

				PLogger.LogError("MenuInteractionNode: " + node.name + " is not a LastChoiceEqualsNode");
				return false;
			});

			if (CurrentChild != targetChild) {
				CurrentChild?.Terminate();
				CurrentChild = targetChild;
			}

			if (targetChild == null) {
				PLogger.LogWarning("MenuInteractionNode: " + this.Context.PendingChoiceId + " not found");
				return State.Running;
			}

			switch (targetChild.Update()) {
				case State.Running:
					return State.Running;

				case State.Failure:
					this.Context.PendingChoiceId = null;
					return State.Failure;

				case State.Success:
					return State.Running;
			}

			return State.Running;
		}
	}
}
