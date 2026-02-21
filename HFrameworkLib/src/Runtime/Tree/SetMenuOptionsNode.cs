using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HFramework.Tree
{
	public class SetMenuOptionsNode : ActionNode
	{
		public List<MenuOption> options = new List<MenuOption>();

		public bool forceOpen = true;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (this.context.MenuSession != null)
			{
				var opts = this.options
					.Where(o => o != null && !string.IsNullOrEmpty(o.Id))
					.Select(o => (o.Id, o.Text, o.Effect))
					.ToArray();
				this.context.MenuSession.SetOptions(opts);

				if (this.forceOpen)
				{
					this.context.MenuSession.Show();
				}
			}

			return State.Success;
		}
	}
}
