using UnityEngine;

namespace HFramework.Tree
{
	public class ScriptAliveNode : DecoratorNode
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
			if (this.context.NpcA?.dead != 0 || this.context.NpcB?.dead != 0)
			{
				return State.Failure;
			}

			Debug.Log("---- ScriptAlive Node UPDATE");
			child.Update();
			return State.Running;
		}
	}
}
