namespace HFramework.Tree
{
	public class MenuInteractionNode : CompositeNode
	{
		public string InitialMenuId = "";

		public string SuccessOnChoiceId = "leave";

		private Node? currentChild;

		protected override void OnStart()
		{
			this.context.PendingChoiceId = this.InitialMenuId;
		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			PLogger.LogDebug("MenuInteractionNode: " + this.context.PendingChoiceId);
			if (this.context.PendingChoiceId == this.SuccessOnChoiceId)
			{
				PLogger.LogDebug("MenuInteractionNode: Success");
				return State.Success;
			}

			if (this.context.PendingChoiceAction != null)
			{
				var actionNode = this.children.Find(node => (node as LastChoiceEqualsNode)?.choiceId == this.context.PendingChoiceAction);
				if (actionNode != null) {
					var actionResult = actionNode.Update();
					switch (actionResult) {
						case State.Running:
							PLogger.LogWarning($"MenuInteractionNode: Unexpected ACTION menu '{this.context.PendingChoiceAction}' moving to Running state -- they should be a single fire operation");
							break;

						case State.Failure:
							PLogger.LogWarning($"MenuInteractionNode: ACTION menu '{this.context.PendingChoiceAction}' failed");
							return State.Failure; // Abort everything
					}
				} else {
					PLogger.LogWarning($"MenuInteractionNode: ACTION menu '{this.context.PendingChoiceAction}' not found");
				}

				this.context.PendingChoiceAction = null;
			}

			if (this.context.PendingChoiceId == null)
			{
				PLogger.LogDebug("MenuInteractionNode: No pending choice");
				return State.Running;
			}

			var targetChild = this.children.Find((node) =>
			{
				if (node is LastChoiceEqualsNode choice)
				{
					return choice.choiceId == this.context.PendingChoiceId;
				}

				PLogger.LogError("MenuInteractionNode: " + node.name + " is not a LastChoiceEqualsNode");
				return false;
			});

			if (currentChild != targetChild)
			{
				currentChild?.Terminate();
				currentChild = targetChild;
			}

			if (targetChild == null)
			{
				PLogger.LogWarning("MenuInteractionNode: " + this.context.PendingChoiceId + " not found");
				return State.Running;
			}

			switch (targetChild.Update())
			{
				case State.Running:
					return State.Running;

				case State.Failure:
					this.context.PendingChoiceId = null;
					return State.Failure;

				case State.Success:
					return State.Running;
			}

			return State.Running;
		}
	}
}
