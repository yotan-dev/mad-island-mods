using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Flow.WaitNode
{
	[Serializable]
	[Experimental]
	[MovedFrom(true, "HFramework.ScriptNodes.WaitNode", null, "WaitTime")]
	public class WaitTime : WaitKind
	{
		public float DurationSeconds;

		private float StartTime;

		public override void Start() {
			StartTime = Time.time;
		}

		public override bool IsDone() {
			return Time.time - StartTime >= DurationSeconds;
		}
	}
}
