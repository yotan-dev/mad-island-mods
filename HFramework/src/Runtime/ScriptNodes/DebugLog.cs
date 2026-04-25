using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Debug/Debug Log")]
	public class DebugLog : Action
	{
		public string Message;

		protected override void OnStart() {
			Debug.Log($"OnStart: {Message}");
		}

		protected override void OnStop() {
			Debug.Log($"OnStop: {Message}");
		}

		protected override State OnUpdate() {
			Debug.Log($"OnUpdate: {Message}");
			return State.Success;
		}
	}
}
