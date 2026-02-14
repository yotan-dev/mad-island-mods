using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HFramework.Tree
{
	public class SetMenuOptionsNode : ActionNode
	{
		public List<MenuOption> options = new List<MenuOption>();

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
					.Select(o => (o.Id, o.Text))
					.ToArray();
				this.context.MenuSession.SetOptions(opts);
			}

			return State.Success;
		}
	}
}
