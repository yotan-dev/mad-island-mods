using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Menu/Toggle Menu")]
	public class ToggleMenu : Action
	{
		public bool ToVisibility = true;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (this.ToVisibility)
				this.context.MenuSession?.Show();
			else
				this.context.MenuSession?.Hide();

			return State.Success;
		}
	}
}
