using HFramework.Scenes;

namespace HFramework.Tree
{
	public class ToggleSexMeter : ActionNode
	{
		public bool Show = true;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (this.Show)
			{
				SexMeter.Instance.Show();
			}
			else
			{
				SexMeter.Instance.Hide();
			}

			return State.Success;
		}
	}
}
