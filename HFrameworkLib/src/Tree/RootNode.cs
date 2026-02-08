namespace HFramework.Tree
{
	public class RootNode : Node
	{
		public Node child;

		public Node teardownNode;

		protected override void OnStart()
		{

		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			return child.Update();
		}

		public override Node Clone(CommonContext context)
		{
			var node = Instantiate(this);
			node.context = context;
			node.child = child.Clone(context);

			return node;
		}
	}
}
