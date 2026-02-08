namespace HFramework.Tree
{
	public class RootNode : Node
	{
		public Node child;

		public Node teardownNode;

		State mainFinalState;

		bool mainFinished;

		protected override void OnStart()
		{
			mainFinalState = State.Running;
			mainFinished = false;
		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			if (!mainFinished)
			{
				var mainState = child?.Update() ?? State.Success;
				if (mainState == State.Running)
				{
					return State.Running;
				}

				mainFinalState = mainState;
				mainFinished = true;
			}

			if (teardownNode != null)
			{
				var teardownState = teardownNode.Update();
				if (teardownState == State.Running)
				{
					return State.Running;
				}
			}

			return mainFinalState;
		}

		public override Node Clone(CommonContext context)
		{
			var node = Instantiate(this);
			node.context = context;
			node.child = child != null ? child.Clone(context) : null;
			node.teardownNode = teardownNode != null ? teardownNode.Clone(context) : null;

			return node;
		}
	}
}
