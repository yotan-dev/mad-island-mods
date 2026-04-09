#nullable enable

using System.Linq;
using HFramework.Scenes;
using UnityEngine;
using YotanModCore;

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

		private void RestoreLivingNpc(CommonStates? character, float? searchAngle)
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

		private void RestoreLivingPlayer(CommonStates? character)
		{
			if (character == null)
				return;

			var mesh = character.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;
		}

		private bool IsPlayerInvolved()
		{
			var currentPlayer = CommonUtils.GetActivePlayer();
			return this.context.Actors.Any(npc => npc.Common == currentPlayer);
		}

		protected override State OnUpdate()
		{
			Debug.Log("Teardown.Update");
			if (this.context.TmpSex != null)
				UnityEngine.Object.Destroy(this.context.TmpSex);

			if (this.context.SexPlace != null)
				this.context.SexPlace.user = null;

			var currentPlayer = CommonUtils.GetActivePlayer();
			foreach (var npc in this.context.Actors)
			{
				if (npc.Common == currentPlayer)
					RestoreLivingPlayer(npc.Common);
				else
					RestoreLivingNpc(npc.Common, npc.Angle);

				// Restores hand items
				// Normally, the item itself is already restored by the above,
				// but this is specially important for items like torches which would otherwise lose their "flame"
				//
				// Officially, CommonSexNPC does not do this, but it is officially bugged
				Managers.mn.randChar.HandItemHide(npc.Common, false);
			}

			if (this.context.HasChangedMainCanvasVisibility)
				Managers.mn.uiMN.MainCanvasView(true);

			if (this.context.HasSexMeter)
				SexMeter.Instance.Hide();

			// Refresh the "status" window if the player has it open
			if (this.IsPlayerInvolved())
				Managers.mn.uiMN.StatusChange(null);

			// @TODO: Moral change
			// npcA.MoralChange(3f);
			// npcB.MoralChange(3f);
			return State.Success;
		}
	}
}
