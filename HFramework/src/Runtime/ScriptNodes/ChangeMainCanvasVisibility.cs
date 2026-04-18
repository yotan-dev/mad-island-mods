using YotanModCore;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public class ChangeMainCanvasVisibility : Action
	{
		public bool ToVisibility = true;

		protected override void OnStart()
		{
			this.context.HasChangedMainCanvasVisibility = true;
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			Managers.mn.uiMN.MainCanvasView(this.ToVisibility);
			return State.Success;
		}
	}
}
