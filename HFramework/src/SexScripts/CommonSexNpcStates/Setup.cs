using HFramework.SexScripts.PrefabCreators;
using UnityEngine;

namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class Setup : BaseState
	{
		[SerializeField] private BasePrefabCreator prefabCreator;

		private readonly BaseState onCompleteState;

		public Setup(BasePrefabCreator prefabCreator, BaseState onCompleteState)
		{
			this.prefabCreator = prefabCreator;
			this.onCompleteState = onCompleteState;
		}

		public override void OnEnter(CommonSexNpcScript ctx)
		{
			var prefab = this.prefabCreator.CreatePrefab(ctx.SexPlacePos.Value);
			if (ctx.SexPlace.user != null)
			{
				PLogger.LogError("Sex place already has a user");
				ctx.ShouldStop = true;
				return;
			}

			ctx.SexPlace.user = prefab;

			prefab.transform.position += new Vector3(0.0f, 0.0f, 0.02f);
			ctx.NpcAAngle = ctx.NpcA.nMove.searchAngle;
			ctx.NpcBAngle = ctx.NpcB.nMove.searchAngle;
			ctx.NpcA.nMove.searchAngle = 180f;
			ctx.NpcB.nMove.searchAngle = 180f;
			ctx.NpcA.gameObject.transform.position = ctx.SexPlacePos.Value;
			ctx.NpcB.gameObject.transform.position = ctx.SexPlacePos.Value;

			this.prefabCreator.SetAppearance(prefab, ctx);
			ctx.TmpSex = prefab;
			ctx.TmpSexAnim = this.prefabCreator.GetSkeletonAnimation(prefab);

			ctx.ChangeState(this.onCompleteState);
		}

		public override void OnExit(CommonSexNpcScript ctx)
		{
			base.OnExit(ctx);
		}

		public override void Update(CommonSexNpcScript ctx)
		{
			base.Update(ctx);
		}
	}
}
