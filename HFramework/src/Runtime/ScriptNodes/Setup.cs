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
		public bool StopNpcReaction = true;

		// CommonSexNPc -> new Vector3(0.0f, 0.0f, 0.02f)
		// commonSexPlayer -> zero
		public Vector3 PositionOffset = Vector3.zero;

		public PrefabPositionSource PositionSource = PrefabPositionSource.SexPlacePos;

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		private bool TryGetPosition(out Vector3 position) {
			position = Vector3.zero;
			switch (this.PositionSource) {
				case PrefabPositionSource.SexPlacePos:
					if (!this.Context.SexPlacePos.HasValue) {
						PLogger.LogWarning("Sex place pos is not set");
						return false;
					}

					position = this.Context.SexPlacePos.Value;
					return true;

				case PrefabPositionSource.Actor0:
					if (this.Context.Actors.Length == 0 || this.Context.Actors[0] == null || this.Context.Actors[0].Common == null) {
						PLogger.LogWarning($"Actor 0 is not set for \"{this.Context.SexScript.name}\".");
						return false;
					}

					position = this.Context.Actors[0].Common.gameObject.transform.position;
					return true;

				case PrefabPositionSource.Actor1:
					if (this.Context.Actors.Length <= 1 || this.Context.Actors[1] == null || this.Context.Actors[1].Common == null) {
						PLogger.LogWarning($"Actor 1 is not set for \"{this.Context.SexScript.name}\".");
						return false;
					}
					position = this.Context.Actors[1].Common.gameObject.transform.position;
					return true;

				default:
					PLogger.LogWarning($"Unknown position source: {this.PositionSource}");
					return false;
			}
		}

		protected override State OnUpdate() {
			// If there is already a TmpSex object, destroy it (e.g. we are changing active "scene")
			if (this.Context.TmpSex != null) {
				GameObject.Destroy(this.Context.TmpSex);
				this.Context.TmpSex = null;
				this.Context.TmpSexAnim = null;
			}

			if (!this.TryGetPosition(out var position)) {
				return State.Failure;
			}

			var prefab = this.Instantiator.CreatePrefab(position);
			if (this.Context.SexPlace != null) {
				if (this.Context.SexPlace.user != null) {
					PLogger.LogError("Sex place already has a user");
					return State.Failure;
				}

				this.Context.SexPlace.user = prefab;
			}

			// Pos offset only in CommonSexNpc
			// CommonSexNpc -> new Vector3(0.0f, 0.0f, 0.02f)
			prefab.transform.position += this.PositionOffset;
			var currentPlayer = CommonUtils.GetActivePlayer();
			foreach (var npc in this.Context.Actors) {
				npc.Angle = npc.Common.nMove.searchAngle;
				if (npc.Common != currentPlayer) {
					npc.Common.nMove.searchAngle = this.StopNpcReaction ? 0f : 180f;

					if (this.Context.SexPlacePos.HasValue) {
						npc.Common.transform.position = this.Context.SexPlacePos.Value;
					}
				}
			}

			this.AppearanceSetter.SetAppearance(prefab, this.Context);
			this.Context.TmpSex = prefab;
			this.Context.TmpSexAnim = this.Instantiator.GetSkeletonAnimation(prefab);

			return State.Success;
		}
	}
}
