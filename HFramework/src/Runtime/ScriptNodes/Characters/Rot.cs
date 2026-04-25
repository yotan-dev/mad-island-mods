using UnityEngine;

namespace HFramework.ScriptNodes.Characters
{
	[ScriptNode("HFramework", "Characters/Rot")]
	public class Rot : Action
	{
		[ActorIndex]
		public int Actor;

		public Vector3 Amount = Vector3.right;

		protected override void OnStart() {

		}

		protected override State OnUpdate() {
			var actor = this.Context.Actors[this.Actor].Common;
			actor.nMove?.Rot(actor.transform.position + this.Amount);

			return State.Success;
		}

		protected override void OnStop() {

		}
	}
}
