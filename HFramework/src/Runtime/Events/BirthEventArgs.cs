#nullable enable

using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class BirthEventArgs : SexEventArgs
	{
		[Serializable]
		public class ChildDetails
		{
			public CommonStates Common;

			public bool AffectedByItems;

			public ChildDetails(CommonStates common, bool affectedByItems) {
				Common = common;
				AffectedByItems = affectedByItems;
			}
		}

		[ActorIndex]
		public int MotherNpcIdx;

		public bool WasBorn;

		public ChildDetails? Child;

		[HideInInspector] public CommonStates? Mother;

		public override void Populate(CommonContext ctx, EmitEvent node) {
			base.Populate(ctx, node);
			Mother = ctx.Actors[MotherNpcIdx].Common;
		}

		internal void CopyInspectorDataFrom(BirthEventArgs other) {
			base.CopyInspectorDataFrom(other);
			this.MotherNpcIdx = other.MotherNpcIdx;
			this.WasBorn = other.WasBorn;
			this.Child = other.Child;
		}

		public override SexEventArgs Clone() {
			var newInstance = new BirthEventArgs();
			newInstance.CopyInspectorDataFrom(this);

			return newInstance;
		}
	}
}
