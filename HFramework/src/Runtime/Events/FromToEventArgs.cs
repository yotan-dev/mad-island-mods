using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using UnityEngine.Serialization;

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class FromToEventArgs : SexEventArgs
	{
		[ActorIndex]
		[FormerlySerializedAs("fromNpcIdx")]
		public int FromNpcIdx;

		[ActorIndex]
		[FormerlySerializedAs("toNpcIdx")]
		public int ToNpcIdx;

		[HideInInspector] public CommonStates From;
		[HideInInspector] public CommonStates To;

		public override void Populate(CommonContext ctx, EmitEvent node) {
			base.Populate(ctx, node);
			From = ctx.Actors[FromNpcIdx].Common;
			To = ctx.Actors[ToNpcIdx].Common;
		}

		internal void CopyInspectorDataFrom(FromToEventArgs other) {
			base.CopyInspectorDataFrom(other);
			this.FromNpcIdx = other.FromNpcIdx;
			this.ToNpcIdx = other.ToNpcIdx;
		}

		public override SexEventArgs Clone() {
			var newInstance = new FromToEventArgs();
			newInstance.CopyInspectorDataFrom(this);

			return newInstance;
		}
	}
}
