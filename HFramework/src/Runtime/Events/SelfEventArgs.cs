using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class SelfEventArgs : SexEventArgs
	{
		[ActorIndex]
		public int fromNpcIdx;
		[HideInInspector] public CommonStates Self;

		public override void Populate(CommonContext ctx, EmitEvent node) {
			base.Populate(ctx, node);
			Self = ctx.Actors[fromNpcIdx].Common;
		}

		internal void CopyInspectorDataFrom(SelfEventArgs other) {
			base.CopyInspectorDataFrom(other);
			this.fromNpcIdx = other.fromNpcIdx;
		}

		public override SexEventArgs Clone() {
			var newInstance = new SelfEventArgs();
			newInstance.CopyInspectorDataFrom(this);

			return newInstance;
		}
	}
}
