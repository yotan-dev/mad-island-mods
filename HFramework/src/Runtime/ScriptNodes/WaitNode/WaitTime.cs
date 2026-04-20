using System;
using UnityEngine;

namespace HFramework.ScriptNodes.WaitNode
{
	[Serializable]
	[Experimental]
	public class WaitTime : WaitKind
	{
		public float DurationSeconds;

		private float _startTime;

		public override void Start() {
			_startTime = Time.time;
		}

		public override bool IsDone() {
			return Time.time - _startTime >= DurationSeconds;
		}
	}
}
