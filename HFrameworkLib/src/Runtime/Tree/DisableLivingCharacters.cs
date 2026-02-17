using UnityEngine;
using YotanModCore;

namespace HFramework.Tree
{
	public class DisableLivingCharacters : ActionNode
	{
		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			var currentPlayer = CommonUtils.GetActivePlayer();
			foreach (var actor in this.context.Actors)
			{
				if (actor.Common == currentPlayer)
				{
					// @TODO: This is based on CommonSexNpc -- needs to check for others
					var mesh = actor.Common.anim.GetComponent<MeshRenderer>();
					if (mesh != null)
						mesh.enabled = false;
					continue;
				} else {
					// @TODO: Valid for CommonSexNpc and CommonSexPlayer
					actor.Common.nMove.RBState(false);
					var coll = actor.Common.GetComponent<CapsuleCollider>();
					if (coll != null)
						coll.enabled = false;

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
