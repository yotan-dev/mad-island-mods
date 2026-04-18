using UnityEngine;

namespace HFramework.ScriptNodes
{
	/// <summary>
	/// Base class for nodes that continually executes their child node.
	/// They may be used to perform something every tick, or ensure something is happening (and breaking the process if needed)
	/// </summary>
	[Experimental]
	public abstract class Passthrough : ScriptNode
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
