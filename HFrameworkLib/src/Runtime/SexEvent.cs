using System;

namespace HFramework
{
	public class SexEvent
	{
		public readonly string id;

		public event EventHandler Triggered;

		public SexEvent(string id)
		{
			this.id = id;
		}

		public void Trigger()
		{
			PLogger.LogError($"Event triggered: {id}");
			Triggered?.Invoke(this, EventArgs.Empty);
		}
	}
}
