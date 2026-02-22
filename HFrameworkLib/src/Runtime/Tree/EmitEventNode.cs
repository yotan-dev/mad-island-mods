using System;
using UnityEngine;

namespace HFramework.Tree
{
	public class EmitEventNode : ActionNode
	{
		public string eventKey;

		public bool EmitOnce = false;

		[SerializeReference]
		public SexEventArgs EventArgs;

		[HideInInspector] public bool hasEmitted = false;

		protected override void OnStart()
		{
			Debug.Log($"OnStart: {eventKey}");
		}

		protected override void OnStop()
		{
			Debug.Log($"OnStop: {eventKey}");
		}

		protected override State OnUpdate()
		{
			Debug.Log($"OnUpdate: {eventKey}");
			if (this.EmitOnce && this.hasEmitted)
			{
				return State.Success;
			}

			if (SexEvents.Events.TryGetValue(eventKey, out var eventInfo))
			{
				try
				{
					this.hasEmitted = true;
					var eventArgs = this.EventArgs.Clone();
					eventArgs.Populate(context, this);
					eventInfo.Event.TriggerWithBaseArgs(eventArgs);
					return State.Success;
				}
				catch (Exception ex)
				{
					Debug.LogError($"Error triggering event {eventKey}: {ex}");
					return State.Failure;
				}
			}

			Debug.LogError($"Event not found: {eventKey}");
			return State.Failure;
		}
	}
}
