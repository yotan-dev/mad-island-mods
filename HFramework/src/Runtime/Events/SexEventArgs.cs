using System;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.Events
{
	[Serializable]
	[Experimental]
	public class SexEventArgs : EventArgs
	{
		/// <summary>
		/// Optional comment to describe the event or provide additional context.
		/// Shown in the inspector for better documentation.
		///
		/// It is actually an workaround because for some reason EmitEventNode_Inspector can't render properly
		/// if this class does not have a property and an Event uses it directly. Dunno why, but a comment field is not bad, right?
		/// </summary>
		[TextArea(1, 3)]
		[Tooltip("Optional comment to describe the event or provide additional context. Shown in the inspector for better documentation.")]
		public string comment;

		[HideInInspector] public CommonContext ctx;

		public virtual void Populate(CommonContext ctx, EmitEvent node) {
			this.ctx = ctx;
		}

		internal void CopyInspectorDataFrom(SexEventArgs other) {
			this.comment = other.comment;
		}

		public virtual SexEventArgs Clone() {
			var newInstance = new SexEventArgs();
			newInstance.CopyInspectorDataFrom(this);
			return newInstance;
		}
	}
}
