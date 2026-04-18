using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Repeat")]
	public class Repeat : Decorator
	{
		protected override void OnStart()
		{
			Debug.Log("---- Repeat Node Start");
		}

		protected override void OnStop()
		{
			Debug.Log("---- Repeat Node Stop");
		}

		protected override State OnUpdate()
		{
			Debug.Log("---- Repeat Node UPDATE");
			child.Update();
			return State.Running;
		}
	}
}
