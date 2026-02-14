using HFramework.Scenes;
using UnityEngine;

namespace HFramework.Tree
{
	public class TickSexMeter : DecoratorNode
	{
		protected override void OnStart()
		{

		}

		protected override void OnStop()
		{

		}

		/// <summary>
		/// Fills the sex meter based on the current sex action.
		/// Kept as a separate method to enable overriding/hooking for custom behavior.
		/// </summary>
		protected virtual void FillSexMeter()
		{
			switch (this.context.SexAction)
			{
				case SexAction.Caressing:
					if (SexMeter.Instance.FillAmount <= SexMeter.Instance.DividerPercent)
						SexMeter.Instance.Fill(Time.deltaTime * 0.03f);
					else
						SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
					break;
				case SexAction.SexSlow:
					if (SexMeter.Instance.FillAmount <= SexMeter.Instance.DividerPercent)
						SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
					else
						SexMeter.Instance.Fill(Time.deltaTime * 0.03f);
					break;
				case SexAction.SexFast:
					if (SexMeter.Instance.FillAmount <= SexMeter.Instance.DividerPercent)
						SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
					else
						SexMeter.Instance.Fill(Time.deltaTime * 0.05f);
					break;
			}
		}

		protected override State OnUpdate()
		{
			this.FillSexMeter();

			// If we don't have a children, nothing we can do.
			if (this.child == null)
			{
				PLogger.LogWarning("TickSexMeter: No child found, this means it will only tick once and stop, which is probably not what you were expecting.");
				return State.Success;
			}

			return this.child.Update();
		}
	}
}
