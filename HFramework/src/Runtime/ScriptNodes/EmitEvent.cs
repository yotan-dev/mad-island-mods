using System;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Emit Event")]
	public class EmitEvent : Action
	{
		public string EventKey;

		public bool EmitOnce = false;

		[SerializeReference]
		public SexEventArgs EventArgs;

		[HideInInspector] public bool hasEmitted = false;

		protected override void OnStart() {
			Debug.Log($"OnStart: {EventKey}");
		}

		protected override void OnStop() {
			Debug.Log($"OnStop: {EventKey}");
		}

		protected override State OnUpdate() {
			Debug.Log($"OnUpdate: {EventKey}");
			if (this.EmitOnce && this.hasEmitted) {
				return State.Success;
			}

			if (SexEvents.Events.TryGetValue(EventKey, out var eventInfo)) {
				try {
					this.hasEmitted = true;
					var eventArgs = this.EventArgs.Clone();
					eventArgs.Populate(Context, this);
					eventInfo.Event.TriggerWithBaseArgs(eventArgs);
					return State.Success;
				} catch (Exception ex) {
					Debug.LogError($"Error triggering event {EventKey}: {ex}");
					return State.Failure;
				}
			}

			Debug.LogError($"Event not found: {EventKey}");
			return State.Failure;
		}
	}
}
