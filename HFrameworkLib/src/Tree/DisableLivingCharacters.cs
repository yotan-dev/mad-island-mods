using UnityEngine;

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
			foreach (var npc in this.context.Npcs)
			{
				npc.Common.nMove.RBState(false);
				var coll = npc.Common.GetComponent<CapsuleCollider>();
				var mesh = npc.Common.anim.GetComponent<MeshRenderer>();
				mesh.enabled = false;
				if (coll != null)
					coll.enabled = false;
			}

			return State.Success;
		}
	}
}
