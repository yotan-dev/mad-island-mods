using HFramework.Scenes;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public class CreateSexMeter : Action
	{
		public float DividerPercent = 0.3f;

		public Vector3 Offset = new(1f, 1f, 0f);

		public bool ShowImmediately = true;

		protected override void OnStart()
		{
			this.context.HasSexMeter = true;
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			if (this.context.SexPlacePos == null)
			{
				PLogger.LogError("SexPlacePos is null");
				return State.Failure;
			}

			SexMeter.Instance.Init(this.context.SexPlacePos.Value + this.Offset, this.DividerPercent);
			if (this.ShowImmediately)
			{
				SexMeter.Instance.Show();
			}

			return State.Success;
		}
	}
}
