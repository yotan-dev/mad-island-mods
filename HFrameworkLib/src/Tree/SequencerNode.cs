using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
	int current;

	protected override void OnStart()
	{
		current = 0;
	}

	protected override void OnStop()
	{

	}

	protected override State OnUpdate()
	{
		var child = this.children[current];
		switch (child.Update())
		{
			case State.Running:
				return State.Running;

			case State.Failure:
				return State.Failure;

			case State.Success:
				current++;
				break;
		}

		return current == children.Count ? State.Success : State.Running;
	}
}
