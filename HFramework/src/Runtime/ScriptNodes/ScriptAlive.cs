using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Script Alive")]
	public class ScriptAlive : Decorator
	{
		protected override void OnStart()
		{
			Debug.Log("---- ScriptAlive Node Start");
		}

		protected override void OnStop()
		{
			Debug.Log("---- ScriptAlive Node Stop");
		}

		protected override State OnUpdate()
		{
			foreach (var npc in this.context.Actors)
			{
				if (npc.Common.dead != 0)
				{
					PLogger.LogError("ScriptAliveNode: Npc is dead");
					return State.Failure;
				}
			}

			Debug.Log("---- ScriptAlive Node UPDATE");
			return child.Update();
		}
	}
}
