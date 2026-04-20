using HFramework.ScriptNodes.WaitNode;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Wait")]
	public class Wait : Action
	{
		[SerializeReference, Subclass]
		public WaitKind WaitKind;

		protected override void OnStart() {
			WaitKind.Start();
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			if (WaitKind.IsDone()) {
				return State.Success;
			}

			return State.Running;
		}
	}
}
