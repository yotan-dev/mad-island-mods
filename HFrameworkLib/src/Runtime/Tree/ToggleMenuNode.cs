using UnityEngine;

namespace HFramework.Tree
{
	public class ToggleMenuNode : ActionNode
	{
		public bool Visible = true;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (this.Visible)
				this.context.MenuSession?.Show();
			else
				this.context.MenuSession?.Hide();

			return State.Success;
		}
	}
}
