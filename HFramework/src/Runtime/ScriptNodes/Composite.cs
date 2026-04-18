using System.Collections.Generic;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public abstract class Composite : ScriptNode
	{
		[HideInInspector] public List<ScriptNode> children = new();

		public override ScriptNode Clone(CommonContext context)
		{
			var node = Instantiate(this);
			node.context = context;
			node.children = children.ConvertAll(c => c.Clone(context));

			return node;
		}

		public override void Terminate(bool fromOutside = true)
		{
			foreach (var child in children)
			{
				child.Terminate(fromOutside);
			}

			base.Terminate(fromOutside);
		}
	}
}
