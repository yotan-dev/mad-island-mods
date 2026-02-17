using UnityEngine;

namespace HFramework.Tree
{
	public class HideMenuNode : ActionNode
	{
		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			this.context.MenuSession?.Hide();
			return State.Success;
		}
	}
}
