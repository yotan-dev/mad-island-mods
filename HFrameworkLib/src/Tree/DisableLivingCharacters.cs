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
			this.context.NpcA.nMove.RBState(false);
			this.context.NpcB.nMove.RBState(false);
			CapsuleCollider aColl = this.context.NpcA.GetComponent<CapsuleCollider>();
			CapsuleCollider bColl = this.context.NpcB.GetComponent<CapsuleCollider>();
			MeshRenderer aMesh = this.context.NpcA.anim.GetComponent<MeshRenderer>();
			MeshRenderer bMesh = this.context.NpcB.anim.GetComponent<MeshRenderer>();
			aMesh.enabled = false;
			bMesh.enabled = false;
			if ((UnityEngine.Object)aColl != (UnityEngine.Object)null)
				aColl.enabled = false;
			if ((UnityEngine.Object)bColl != (UnityEngine.Object)null)
				bColl.enabled = false;

			return State.Success;
		}
	}
}
