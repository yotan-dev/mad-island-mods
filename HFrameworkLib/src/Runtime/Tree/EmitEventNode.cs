using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFramework.Tree
{
	public class EmitEventNode : ActionNode
	{
		public string eventKey;

		[SerializeReference]
		public BaseSexEventArgs EventArgs;

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

			if (SexEvents.Events.TryGetValue(eventKey, out var eventInfo))
			{
				try
				{
					var eventArgs = Activator.CreateInstance(eventInfo.EventType) as BaseSexEventArgs;
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
