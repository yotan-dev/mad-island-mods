using UnityEngine;

namespace HFramework.ScriptNodes.Other
{
	/// <summary>
	/// Handle the player-raped battle progression.
	/// - Success if player is defeated
	/// - Failure if player escapes
	/// </summary>
	[ScriptNode("HFramework", "Other/Player Raped Battle")]
	public class PlayerRapedBattle : Action
	{
		[ActorIndex]
		public int PlayerIndex = 1;

		private float FaintTime = 1f;

		private CommonStates PlayerCommon = null;

		protected override void OnStart() {
			FaintTime = 1f;
			PlayerCommon = this.context.Actors[PlayerIndex].Common;
		}

		protected override State OnUpdate() {
			if (PlayerCommon.faint > 0.0 && this.context.TmpSex != null && PlayerCommon.pMove.common.dead != 0) {
				this.FaintTime -= Time.deltaTime;
				if (this.FaintTime <= 0.0) {
					this.FaintTime = 1f;
					PlayerCommon.FaintChange(-5.0, view: true, grapple: true);
				}

				return State.Running;
			}

			// Player escaped battle, fail the script
			if (PlayerCommon.pMove.common.dead == 0) {
				return State.Failure;
			}

			// Player was defeated, success
			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
