using YotanModCore;

namespace HFramework.ScriptNodes
{
	/// <summary>
	/// Marks a section that will be skipped if the player presses the skip button.
	///
	/// Once clicked, remaining children will be skipped, unless they are "DontSkipSequence" nodes,
	/// those nodes will be run til the end.
	///
	/// Once all children have either been processed (DontSkipSequence) or skipped, this node will succeed.
	/// If a children fails, this node fails.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Flow/Skippable Section")]
	public class SkippableSection : Sequence
	{
		private bool _hasSkipped = false;

		protected override void OnStart() {
			base.OnStart();
			this._hasSkipped = false;
			Managers.mn.uiMN.SkipView(true);
		}

		protected override void OnStop() {
			base.OnStop();
			Managers.mn.uiMN.SkipView(false);
		}

		protected override State OnUpdate() {
			if (Managers.mn.uiMN.skip) {
				_hasSkipped = true;
			}

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
			while (this.Current < this.Children.Count) {
				var child = this.Children[this.Current];

				if (this._hasSkipped) {
					// We are in skip mode, everythig will be skipped unless it is
					// a DontSkipSequence node
					if (child is not DontSkipSequence) {
						// Terminate the child if it is running
						if (child.Started) {
							child.Terminate();
						}

						this.Current++;
						continue;
					}
				}

				switch (child.Update()) {
					case State.Running:
						return State.Running;

					case State.Failure:
						return State.Failure;

					case State.Success:
						this.Current++;
						break;
						/* continue to next child */
				}
			}

			return State.Success;
		}
	}
}
