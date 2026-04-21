using UnityEngine;
using YotanModCore;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Change Main Canvas Visibility")]
	public class ChangeMainCanvasVisibility : Action
	{
		[Tooltip("Expected visibility state of the main canvas")]
		public bool ToVisibility = true;

		protected override void OnStart() {
			this.Context.HasChangedMainCanvasVisibility = true;
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			Managers.mn.uiMN.MainCanvasView(this.ToVisibility);
			return State.Success;
		}
	}
}
