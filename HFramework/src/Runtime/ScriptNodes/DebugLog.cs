using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Debug/Debug Log")]
	public class DebugLog : Action
	{
		public string message;

		protected override void OnStart()
		{
			Debug.Log($"OnStart: {message}");
		}

		protected override void OnStop()
		{
			Debug.Log($"OnStop: {message}");
		}

		protected override State OnUpdate()
		{
			Debug.Log($"OnUpdate: {message}");
			return State.Success;
		}
	}
}
