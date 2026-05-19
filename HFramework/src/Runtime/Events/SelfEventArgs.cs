using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using UnityEngine.Serialization;

// Temporary since FromNpcIdx is obsolete (Remove once we erase)
#pragma warning disable 0618

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class SelfEventArgs : SexEventArgs
	{
		[ActorIndex]
		[FormerlySerializedAs("fromNpcIdx")]
		[Obsolete("Use ActorsIdx instead")]
		public int FromNpcIdx;

		[ActorIndex]
		public int[] ActorsIdx;

		[HideInInspector] public CommonStates Self;

		[HideInInspector] public CommonStates[] Actors;

		public override void Populate(CommonContext ctx, EmitEvent node) {
			base.Populate(ctx, node);
			Self = ctx.Actors[FromNpcIdx].Common;

			if (ActorsIdx != null) {
				Actors = new CommonStates[ActorsIdx.Length];
				for (int i = 0; i < ActorsIdx.Length; i++) {
					Actors[i] = ctx.Actors[ActorsIdx[i]].Common;
				}
			}
		}

		internal void CopyInspectorDataFrom(SelfEventArgs other) {
			base.CopyInspectorDataFrom(other);
			this.FromNpcIdx = other.FromNpcIdx;
			this.ActorsIdx = other.ActorsIdx;
		}

		public override SexEventArgs Clone() {
			var newInstance = new SelfEventArgs();
			newInstance.CopyInspectorDataFrom(this);

			return newInstance;
		}
	}
}

// Restore warning for other code
#pragma warning restore 0618
