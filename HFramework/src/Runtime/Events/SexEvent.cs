using System;
using UnityEngine.Serialization;

namespace HFramework.Events
{
	[Experimental]
	public class SexEvent<T> : ISexEvent<T> where T : SexEventArgs, new()
	{
		[FormerlySerializedAs("id")]
		public readonly string Id;

		public event EventHandler<T> Triggered;

		public SexEvent(string id) {
			this.Id = id;
		}

		public void Trigger(T args) {
			// Implementation here
			PLogger.LogError($"Event triggered: {Id}");
			Triggered?.Invoke(this, args);
		}

		void ISexEventBase.TriggerWithBaseArgs(SexEventArgs args) {
			if (args is T typedArgs) {
				Trigger(typedArgs);
			} else {
				throw new ArgumentException($"Expected argument of type {typeof(T).Name}, but got {args.GetType().Name}");
			}
		}

		public string GetId() {
			return Id;
		}
	}
}
