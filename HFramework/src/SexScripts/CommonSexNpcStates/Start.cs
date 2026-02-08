namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class Start : BaseState
	{
		public override void OnEnter(CommonSexNpcScript ctx)
		{
			NPCMove aMove = ctx.NpcA.GetComponent<NPCMove>();
			NPCMove bMove = ctx.NpcB.GetComponent<NPCMove>();
			aMove.actType = NPCMove.ActType.Wait;
			bMove.actType = NPCMove.ActType.Wait;
			ctx.NpcA.sex = CommonStates.SexState.Playing;
			ctx.NpcB.sex = CommonStates.SexState.Playing;

			ctx.ChangeState(ctx.States.MoveToPlace);
		}
	}
}
