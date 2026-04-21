#nullable enable

using System.Collections;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes
{
	/// <summary>
	/// Teardown node for player-raped encounters.
	/// This is very specific to match the game special behavior.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Flow/PlayerRaped Teardown")]
	public class PlayerRapedTeardown : Action
	{
		private IEnumerator? FadeCoroutine;

		protected override void OnStart() {
			FadeCoroutine = null;
			if (this.Context.MainNodeState == ScriptNode.State.Success) {
				FadeCoroutine = Managers.mn.eventMN.FadeOut();
			}
		}

		protected override void OnStop() {
		}

		private void RestoreLivingCharacter(CommonStates? character) {
			if (character == null)
				return;

			if (character != CommonUtils.GetActivePlayer()) {
				if (this.Context.MainNodeState != State.Success) {
					// Player mesh is only restored at this moment if the rape was a failure.
					// On success, game will restore the player later, during the revive step.
					var mesh = character.anim.GetComponent<MeshRenderer>();
					if (mesh != null)
						mesh.enabled = true;
				}
			} else {
				// NPC mesh is always restored
				var mesh = character.anim.GetComponent<MeshRenderer>();
				if (mesh != null)
					mesh.enabled = true;
			}

			var col = character.GetComponent<CapsuleCollider>();
			if (col != null)
				col.enabled = true;

			var move = character.nMove;
			if (move?.actType != null)
				move.actType = NPCMove.ActType.Interval;
		}

		protected override State OnUpdate() {
			if (this.Context.HasChangedSkipButtonVisibility)
				Managers.mn.uiMN.SkipView(false);

			if (FadeCoroutine != null && FadeCoroutine.MoveNext()) {
				return State.Running;
			}

			if (this.Context.TmpSex != null)
				UnityEngine.Object.Destroy(this.Context.TmpSex);

			foreach (var npc in this.Context.Actors) {
				RestoreLivingCharacter(npc.Common);

				// Restores hand items
				// Normally, the item itself is already restored by the above,
				// but this is specially important for items like torches which would otherwise lose their "flame"
				//
				// Officially, CommonSexNPC does not do this, but it is officially bugged
				Managers.mn.randChar.HandItemHide(npc.Common, false);
			}

			SexEvents.OnEnd?.Trigger(new SexEventArgs() {
				ctx = this.Context,
			});

			// We intentionally don't restore the canvas here -- it will be done by the end event
			// this is an edge case due to the "game-over"-like sequence.
			// if (this.context.HasChangedMainCanvasVisibility) {
			// 	Managers.mn.uiMN.MainCanvasView(true);
			// }

			return State.Success;
		}
	}
}
