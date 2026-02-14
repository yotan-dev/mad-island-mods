using UnityEngine;

namespace HFramework.Tree
{
	public class ShowMenuNode : ActionNode
	{
		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			this.context.MenuSession?.Show();
			return State.Success;
		}
	}
}
