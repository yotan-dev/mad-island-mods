using HFramework.Scenes;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public class ToggleSexMeter : Action
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
