using HFramework.Scenes;
using UnityEngine;

namespace HFramework.Tree
{
	public class CreateSexMeter : ActionNode
	{
		public float DividerPercent = 0.3f;

		public Vector3 Offset = new Vector3(1f, 1f, 0f);

		public bool Show = true;

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
			if (this.Show)
			{
				SexMeter.Instance.Show();
			}

			return State.Success;
		}
	}
}
