using System.Collections.Generic;
using UnityEngine;

namespace HFramework.Tree
{
	public abstract class CompositeNode : Node
	{
		[HideInInspector] public List<Node> children = new List<Node>();

		public override Node Clone(CommonContext context)
		{
			var node = Instantiate(this);
			node.context = context;
			node.children = children.ConvertAll(c => c.Clone(context));

			return node;
		}
	}
}
