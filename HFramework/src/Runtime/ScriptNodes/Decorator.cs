using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public abstract class Decorator : ScriptNode
	{
		[HideInInspector] public ScriptNode child;

		public override ScriptNode Clone(CommonContext context)
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
