namespace HFramework.Tree
{
	public class RootNode : Node
	{
		public Node child;

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

		public override Node Clone()
		{
			var node = Instantiate(this);
			node.child = child.Clone();

			return node;
		}
	}
}
