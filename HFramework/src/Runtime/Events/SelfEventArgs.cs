using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using UnityEngine.Serialization;

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class SelfEventArgs : SexEventArgs
	{
		[ActorIndex]
		[FormerlySerializedAs("fromNpcIdx")]
		public int FromNpcIdx;
		[HideInInspector] public CommonStates Self;

		public override void Populate(CommonContext ctx, EmitEvent node) {
			base.Populate(ctx, node);
			Self = ctx.Actors[FromNpcIdx].Common;
		}

		internal void CopyInspectorDataFrom(SelfEventArgs other) {
			base.CopyInspectorDataFrom(other);
			this.FromNpcIdx = other.FromNpcIdx;
		}

		public override SexEventArgs Clone() {
			var newInstance = new SelfEventArgs();
			newInstance.CopyInspectorDataFrom(this);

			return newInstance;
		}
	}
}
