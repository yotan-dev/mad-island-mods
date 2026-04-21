using System;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Characters/Disable Living Characters")]
	public class DisableLivingCharacters : Action
	{
		[Flags]
		public enum DisableMode
		{
			DisableRigidbody = 1 << 0,
			DisableCollider = 1 << 1,
			HideMesh = 1 << 2,
		}

		[Serializable]
		public class DisableTarget
		{
			public int ActorIndex = 0;

			[Tooltip("Which components to disable")]
			public DisableMode Mode = DisableMode.DisableRigidbody | DisableMode.DisableCollider | DisableMode.HideMesh;
		}

		public DisableTarget[] Targets = new DisableTarget[0];

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			foreach (var target in this.Targets) {
				var actor = this.context.Actors[target.ActorIndex];
				if (actor == null) {
					continue;
				}

				if (target.Mode.HasFlag(DisableMode.DisableRigidbody))
					actor.Common.nMove.RBState(false);

				if (target.Mode.HasFlag(DisableMode.DisableCollider)) {
					var coll = actor.Common.GetComponent<CapsuleCollider>();
					if (coll != null)
						coll.enabled = false;
				}

				if (target.Mode.HasFlag(DisableMode.HideMesh)) {
					var mesh = actor.Common.anim.GetComponent<MeshRenderer>();
					if (mesh != null)
						mesh.enabled = false;
				}

				// Hides hand items
				// Normally, the item itself is already hidden by the above,
				// but this is specially important for items like torches which would otherwise keep their "flame"
				//
				// Officially, CommonSexNPC does not do this, but it is officially bugged
				Managers.mn.randChar.HandItemHide(actor.Common, true);
			}

			return State.Success;
		}
	}
}
