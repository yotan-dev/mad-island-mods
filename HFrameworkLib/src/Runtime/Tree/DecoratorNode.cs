using UnityEngine;

namespace HFramework.Tree
{
	public abstract class DecoratorNode : Node
	{
		[HideInInspector] public Node child;

		public override Node Clone(CommonContext context)
		{
			var node = Instantiate(this);
			node.context = context;
			node.child = child.Clone(context);

			return node;
		}

		public override void Terminate(bool fromOutside = true)
		{
			child.Terminate(fromOutside);
			base.Terminate(fromOutside);
		}
	}
}
