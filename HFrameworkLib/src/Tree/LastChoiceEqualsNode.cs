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
				return State.Failure;

			if (this.context.PendingChoiceId != this.choiceId)
				return State.Failure;

			return this.child != null ? this.child.Update() : State.Success;
		}
	}
}
