using System;

namespace HFramework.Events
{
	[Experimental]
	public class SexEventInfo
	{
		public ISexEventBase Event { get; private set; }
		public Type EventType { get; private set; }

		public void SetEvent<T>(ISexEvent<T> evt) where T : SexEventArgs, new()
		{
			Event = evt;
			EventType = typeof(T);
		}
	}
}
