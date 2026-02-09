using UnityEngine;

namespace HFramework.Tree
{
	public class Setup : ActionNode
	{
		public PrefabConfig prefabConfig;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			var prefab = this.prefabConfig.CreatePrefab(this.context.SexPlacePos.Value);
			if (this.context.SexPlace.user != null)
			{
				PLogger.LogError("Sex place already has a user");
				return State.Failure;
			}

			this.context.SexPlace.user = prefab;

			prefab.transform.position += new Vector3(0.0f, 0.0f, 0.02f);
			this.context.NpcAAngle = this.context.NpcA.nMove.searchAngle;
			this.context.NpcBAngle = this.context.NpcB.nMove.searchAngle;
			this.context.NpcA.nMove.searchAngle = 180f;
			this.context.NpcB.nMove.searchAngle = 180f;
			this.context.NpcA.gameObject.transform.position = this.context.SexPlacePos.Value;
			this.context.NpcB.gameObject.transform.position = this.context.SexPlacePos.Value;

			this.prefabConfig.SetAppearance(prefab, this.context);
			this.context.TmpSex = prefab;
			this.context.TmpSexAnim = this.prefabConfig.GetSkeletonAnimation(prefab);

			return State.Success;
		}
	}
}
