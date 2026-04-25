using HFramework.SexScripts.ScriptContext;
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
		[HideInInspector] public ScriptNode Child;

		public override ScriptNode Clone(CommonContext context) {
			var node = Instantiate(this);
			node.Context = context;
			node.Child = Child.Clone(context);

			return node;
		}

		public override void Terminate(bool fromOutside = true) {
			Child.Terminate(fromOutside);
			base.Terminate(fromOutside);
		}
	}
}
