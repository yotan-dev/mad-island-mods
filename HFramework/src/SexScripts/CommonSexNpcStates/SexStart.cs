namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class SexStart : BaseState
	{
		public override void Update(CommonSexNpcScript ctx)
		{
			// @TODO: Emit Penetration event.

			ctx.ChangeState(ctx.States.SexSpeed1);
		}
	}
}
