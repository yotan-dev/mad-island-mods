using UnityEngine;

namespace HFramework.Tree
{
	public class LastChoiceEqualsNode : DecoratorNode
	{
		public string choiceId = "";

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (string.IsNullOrEmpty(this.choiceId))
			{
				PLogger.LogError("LastChoiceEqualsNode: choiceId is empty");
				return State.Failure;
			}

			if (this.context.PendingChoiceId != this.choiceId && this.context.PendingChoiceAction != this.choiceId)
			{
				PLogger.LogError($"LastChoiceEqualsNode: Triggered a Choice Node that was not expected. Expected choice '{this.choiceId}' but got '{this.context.PendingChoiceId}' or action '{this.context.PendingChoiceAction}'");
				return State.Failure;
			}

			return this.child != null ? this.child.Update() : State.Success;
		}
	}
}
