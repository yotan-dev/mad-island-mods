using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class FromToEventArgs : SexEventArgs
	{
		[ActorIndex]
		public int fromNpcIdx;

		[ActorIndex]
		public int toNpcIdx;

		public bool isRape;

		[HideInInspector] public CommonStates From;
		[HideInInspector] public CommonStates To;

		public override void Populate(CommonContext ctx, EmitEvent node) {
			base.Populate(ctx, node);
			From = ctx.Actors[fromNpcIdx].Common;
			To = ctx.Actors[toNpcIdx].Common;
		}

		internal void CopyInspectorDataFrom(FromToEventArgs other) {
			base.CopyInspectorDataFrom(other);
			this.fromNpcIdx = other.fromNpcIdx;
			this.toNpcIdx = other.toNpcIdx;
			this.isRape = other.isRape;
		}

		public override SexEventArgs Clone() {
			var newInstance = new FromToEventArgs();
			newInstance.CopyInspectorDataFrom(this);

			return newInstance;
		}
	}
}
