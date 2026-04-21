namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Sequence")]
	public class Sequence : Composite
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
			// When a node successes we want the next one to be immediately executed,
			// if we returned Running first, it would wait for the next Tick.
			// This has a few reasons/implications:
			// - If you have 2 nodes that needs to be executed together to prevent the game from going crazy,
			//   this is awesome and allows it.
			// - Not waiting the next tick makes the scripts feel more responsive (and will overall work closely to original code)
			// - On the other hand, this might cause some unexpected node behavior -- but should be rare --
			//   One example was the Wait node, when waiting for Input in a sequence, if we simply Succeed there
			//   the next node would execute immediately and succeed too. In these cases, it is the node's responsibility
			//   to put an artificial tick delay to handle this correctly.
			while (current < this.children.Count) {
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
						/* continue to next child */
				}
			}

			return State.Success;
		}
	}
}
