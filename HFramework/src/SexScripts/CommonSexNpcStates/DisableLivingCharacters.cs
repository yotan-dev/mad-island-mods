using UnityEngine;

namespace HFramework.SexScripts.CommonSexNpcStates
{
	public class DisableLivingCharacters : BaseState
	{
		private BaseState next;

		public DisableLivingCharacters(BaseState next)
		{
			this.next = next;
		}

		public override void Update(CommonSexNpcScript ctx)
		{
			ctx.NpcA.nMove.RBState(false);
			ctx.NpcB.nMove.RBState(false);
			CapsuleCollider aColl = ctx.NpcA.GetComponent<CapsuleCollider>();
			CapsuleCollider bColl = ctx.NpcB.GetComponent<CapsuleCollider>();
			MeshRenderer aMesh = ctx.NpcA.anim.GetComponent<MeshRenderer>();
			MeshRenderer bMesh = ctx.NpcB.anim.GetComponent<MeshRenderer>();
			aMesh.enabled = false;
			bMesh.enabled = false;
			if ((UnityEngine.Object)aColl != (UnityEngine.Object)null)
				aColl.enabled = false;
			if ((UnityEngine.Object)bColl != (UnityEngine.Object)null)
				bColl.enabled = false;

			ctx.ChangeState(this.next);
		}
	}
}
