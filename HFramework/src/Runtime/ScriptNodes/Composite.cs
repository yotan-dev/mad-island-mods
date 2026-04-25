using System.Collections.Generic;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public abstract class Composite : ScriptNode
	{
		[HideInInspector] public List<ScriptNode> Children = new();

		public override ScriptNode Clone(CommonContext context) {
			var node = Instantiate(this);
			node.Context = context;
			node.Children = Children.ConvertAll(c => c.Clone(context));

			return node;
		}

		public override void Terminate(bool fromOutside = true) {
			foreach (var child in Children) {
				child.Terminate(fromOutside);
			}

			base.Terminate(fromOutside);
		}
	}
}
