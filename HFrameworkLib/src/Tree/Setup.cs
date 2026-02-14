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
			prefab.transform.position += new Vector3(0.0f, 0.0f, 0.02f);
			foreach (var npc in this.context.Npcs)
			{
				npc.Angle = npc.Common.nMove.searchAngle;
				npc.Common.nMove.searchAngle = 0f; //180f;
				npc.Common.gameObject.transform.position = this.context.SexPlacePos.Value;
			}

			this.prefabConfig.SetAppearance(prefab, this.context);
			this.context.TmpSex = prefab;
			this.context.TmpSexAnim = this.prefabConfig.GetSkeletonAnimation(prefab);

			return State.Success;
		}
	}
}
