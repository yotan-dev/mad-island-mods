#nullable enable

using UnityEngine;

namespace HFramework.Tree
{
	public class TeardownNode : ActionNode
	{
		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		private void RestoreLivingCharacter(CommonStates? character, float? searchAngle)
		{
			if (character == null)
				return;

			var col = character.GetComponent<CapsuleCollider>();
			if (col != null)
				col.enabled = true;

			var mesh = character.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			var move = character.nMove;
			if (move.actType == NPCMove.ActType.Wait)
				move.actType = NPCMove.ActType.Travel;
			else
				move.actType = NPCMove.ActType.Interval;

			if (searchAngle != null)
				move.searchAngle = searchAngle.Value;
			character.sex = CommonStates.SexState.None;
			if (character.debuff.perfume <= 0.0)
				character.libido -= 20f;
		}

		protected override State OnUpdate()
		{
			Debug.Log("Teardown.Update");
			if (this.context.TmpSex != null)
				UnityEngine.Object.Destroy(this.context.TmpSex);

			if (this.context.SexPlace != null)
				this.context.SexPlace.user = null;

			foreach (var npc in this.context.Npcs)
			{
				RestoreLivingCharacter(npc.Common, npc.Angle);
			}

			// @TODO: Moral change
			// npcA.MoralChange(3f);
			// npcB.MoralChange(3f);
			return State.Success;
		}
	}
}
