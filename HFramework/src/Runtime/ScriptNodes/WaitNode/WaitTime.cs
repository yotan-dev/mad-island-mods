using System;
using UnityEngine;

namespace HFramework.ScriptNodes.WaitNode
{
	[Serializable]
	[Experimental]
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
