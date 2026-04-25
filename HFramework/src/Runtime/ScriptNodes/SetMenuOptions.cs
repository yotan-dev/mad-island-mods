using System.Collections.Generic;
using System.Linq;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Menu/Set Menu Options")]
	public class SetMenuOptions : Action
	{
		public List<MenuOption> Options = new();

		public bool ForceOpen = true;

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			if (this.Context.MenuSession != null) {
				var opts = this.Options
					.Where(o => o != null && !string.IsNullOrEmpty(o.Id))
					.Select(o => (o.Id, o.Text, o.Effect))
					.ToArray();
				this.Context.MenuSession.SetOptions(opts);

				if (this.ForceOpen) {
					this.Context.MenuSession.Show();
				}
			}

			return State.Success;
		}
	}
}
