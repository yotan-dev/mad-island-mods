using System;
using System.Collections.Generic;
using HFramework.Events;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Emit Event")]
	public class EmitEvent : Action
	{
		[Serializable]
		public class EventEntry
		{
			public string EventKey;

			[SerializeReference]
			public SexEventArgs EventArgs;
		}

		public string EventKey;

		[Obsolete]
		public bool EmitOnce = false;

		[SerializeReference]
		public SexEventArgs EventArgs;

		[Obsolete]
		[HideInInspector] public bool hasEmitted = false;

		[SerializeField]
		public List<EventEntry> Events = new();

		protected override void OnStart() {
			Debug.Log($"OnStart: {EventKey}");
		}

		protected override void OnStop() {
			Debug.Log($"OnStop: {EventKey}");
		}

		protected override State OnUpdate() {
			// Legacy version
			if (this.EventKey != "HF.noop") {
				PLogger.LogWarning("Legacy EmitEvent is deprecated, use the array version instead. Once migrated, put the old version to Noop");
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

			// New version
			foreach (var entry in this.Events) {
				if (entry.EventArgs == null) {
					continue;
				}

				if (!SexEvents.Events.TryGetValue(entry.EventKey, out var eventInfo)) {
					try {
						var eventArgs = entry.EventArgs.Clone();
						eventArgs.Populate(Context, this);
						eventInfo.Event.TriggerWithBaseArgs(eventArgs);
					} catch (Exception ex) {
						Debug.LogError($"Error triggering event {entry.EventKey}: {ex}");
						return State.Failure;
					}
				}
			}

			return State.Success;
		}

		protected override void OnValidate() {
			base.OnValidate();
			if (this.EventKey != "HF.noop") {
				var msg = $"Legacy EmitEvent is deprecated, use the array version instead. Once migrated, put the old version to Noop.\nNode: {this.ID}";
				Debug.LogWarning(msg);
			}
		}

		public override bool HasWarnings() {
			return this.EventKey != "HF.noop";
		}

		public void RemoveEvent(string eventKey) {
			if (!this.TryRemoveEvent(eventKey)) {
				throw new System.Exception($"Event not found: {eventKey}");
			}
		}

		public bool TryRemoveEvent(string eventKey) {
			return this.Events.RemoveAll(entry => entry.EventKey == eventKey) > 0;
		}
	}
}
