using HFramework.ScriptNodes.PrefabSetup;
using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Setup")]
	public class Setup : Action
	{
		public enum PrefabPositionSource
		{
			SexPlacePos,
			Actor0,
			Actor1,
		}

		[SerializeReference, Subclass]
		public PrefabInstantiator Instantiator;

		[SerializeReference, Subclass]
		public AppearanceSetter AppearanceSetter;

		// Note: When false, NPC will change out of WAIT and fall down from the world if the sex doesn't stop.
		// CommonSexNpc -> false
		// CommonSexPlayer -> true
		public bool stopNpcReaction = true;

		// CommonSexNPc -> new Vector3(0.0f, 0.0f, 0.02f)
		// commonSexPlayer -> zero
		public Vector3 posOffset = Vector3.zero;

		public PrefabPositionSource PositionSource = PrefabPositionSource.SexPlacePos;

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		private bool TryGetPosition(out Vector3 position) {
			position = Vector3.zero;
			switch (this.PositionSource) {
				case PrefabPositionSource.SexPlacePos:
					if (!this.context.SexPlacePos.HasValue) {
						PLogger.LogWarning("Sex place pos is not set");
						return false;
					}

					position = this.context.SexPlacePos.Value;
					return true;

				case PrefabPositionSource.Actor0:
					if (this.context.Actors.Length == 0 || this.context.Actors[0] == null || this.context.Actors[0].Common == null) {
						PLogger.LogWarning($"Actor 0 is not set for \"{this.context.SexScript.name}\".");
						return false;
					}

					position = this.context.Actors[0].Common.gameObject.transform.position;
					return true;

				case PrefabPositionSource.Actor1:
					if (this.context.Actors.Length <= 1 || this.context.Actors[1] == null || this.context.Actors[1].Common == null) {
						PLogger.LogWarning($"Actor 1 is not set for \"{this.context.SexScript.name}\".");
						return false;
					}
					position = this.context.Actors[1].Common.gameObject.transform.position;
					return true;

				default:
					PLogger.LogWarning($"Unknown position source: {this.PositionSource}");
					return false;
			}
		}

		protected override State OnUpdate() {
			// If there is already a TmpSex object, destroy it (e.g. we are changing active "scene")
			if (this.context.TmpSex != null) {
				GameObject.Destroy(this.context.TmpSex);
				this.context.TmpSex = null;
				this.context.TmpSexAnim = null;
			}

			if (!this.TryGetPosition(out var position)) {
				return State.Failure;
			}

			var prefab = this.Instantiator.CreatePrefab(position);
			if (this.context.SexPlace != null) {
				if (this.context.SexPlace.user != null) {
					PLogger.LogError("Sex place already has a user");
					return State.Failure;
				}

				this.context.SexPlace.user = prefab;
			}

			// Pos offset only in CommonSexNpc
			// CommonSexNpc -> new Vector3(0.0f, 0.0f, 0.02f)
			prefab.transform.position += this.posOffset;
			var currentPlayer = CommonUtils.GetActivePlayer();
			foreach (var npc in this.context.Actors) {
				npc.Angle = npc.Common.nMove.searchAngle;
				if (npc.Common != currentPlayer) {
					npc.Common.nMove.searchAngle = this.stopNpcReaction ? 0f : 180f;
					npc.Common.gameObject.transform.position = this.context.SexPlacePos.Value;
				}
			}

			this.AppearanceSetter.SetAppearance(prefab, this.context);
			this.context.TmpSex = prefab;
			this.context.TmpSexAnim = this.Instantiator.GetSkeletonAnimation(prefab);

			return State.Success;
		}
	}
}
