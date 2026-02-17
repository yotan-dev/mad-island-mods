using YotanModCore;

namespace HFramework.Tree
{
	public class ChangeMainCanvasVisibility : ActionNode
	{
		public bool Visible = true;

		protected override void OnStart()
		{
			this.context.HasChangedMainCanvasVisibility = true;
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			Managers.mn.uiMN.MainCanvasView(this.Visible);
			return State.Success;
		}
	}
}
