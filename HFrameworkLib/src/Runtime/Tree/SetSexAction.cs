namespace HFramework.Tree
{
	public class SetSexAction : ActionNode
	{
		public SexAction Action;

		protected override void OnStart()
		{

		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			this.context.SexAction = this.Action;
			return State.Success;
		}
	}
}
