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

		private bool Completed = false;

		protected override void OnStart() {
			this.Completed = false;
			WaitKind.Start();
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			if (this.Completed) {
				return State.Success;
			}

			if (WaitKind.IsDone()) {
				/**
				 * Give one last running response to allow a tick to happen,
				 * Which will success in the next tick.
				 *
				 * This is important to give the system some space to breath and process other coroutines or
				 * free up the input system. Otherwise multiple inputs in a sequence may all happen at same time.
				 */
				this.Completed = true;
				return State.Running;
			}

			return State.Running;
		}
	}
}
