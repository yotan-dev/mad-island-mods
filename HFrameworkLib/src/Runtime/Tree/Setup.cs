using UnityEngine;
using YotanModCore;

namespace HFramework.Tree
{
	public class Setup : ActionNode
	{
		public PrefabConfig prefabConfig;

		// Note: When false, NPC will change out of WAIT and fall down from the world if the sex doesn't stop.
		// CommonSexNpc -> false
		// CommonSexPlayer -> true
		public bool stopNpcReaction = true;

		// CommonSexNPc -> new Vector3(0.0f, 0.0f, 0.02f)
		// commonSexPlayer -> zero
		public Vector3 posOffset = Vector3.zero;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			var prefab = this.prefabConfig.CreatePrefab(this.context.SexPlacePos.Value);
			if (this.context.SexPlace != null)
			{
				if (this.context.SexPlace.user != null)
				{
					PLogger.LogError("Sex place already has a user");
					return State.Failure;
				}

				this.context.SexPlace.user = prefab;
			}

			// Pos offset only in CommonSexNpc
			// CommonSexNpc -> new Vector3(0.0f, 0.0f, 0.02f)
			prefab.transform.position += this.posOffset;
			var currentPlayer = CommonUtils.GetActivePlayer();
			foreach (var npc in this.context.Actors)
			{
				npc.Angle = npc.Common.nMove.searchAngle;
				if (npc.Common != currentPlayer)
				{
					npc.Common.nMove.searchAngle = this.stopNpcReaction ? 0f : 180f;
					npc.Common.gameObject.transform.position = this.context.SexPlacePos.Value;
				}
			}

			this.prefabConfig.SetAppearance(prefab, this.context);
			this.context.TmpSex = prefab;
			this.context.TmpSexAnim = this.prefabConfig.GetSkeletonAnimation(prefab);

			return State.Success;
		}
	}
}
