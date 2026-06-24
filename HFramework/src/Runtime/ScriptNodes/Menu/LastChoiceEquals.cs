using HFramework.ScriptNodes.Flow;
using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Menu
{
	/// <summary>
	/// Node to handle menu choices. It must be a direct children of MenuInteraction node.
	///
	/// MenuInteraction will trigger this node if the last choice was the specified choice.
	///
	/// This Node will run all of its children in sequence (just like a Sequence node).
	///
	/// At each update cycle, the current children is called and:
	/// - If the child is still running, it will wait for the next tick and recall the same child.
	/// - If the child succeeds, the next one is picked immediately.
	/// - If the child fails, the failure will be propagated to the parent node.
	///
	/// Once all children have succeeded, this node will succeed.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Menu/Last Choice Equals")]
	[MovedFrom(true, "HFramework.ScriptNodes", null, "LastChoiceEquals")]
	public class LastChoiceEquals : Sequence
	{
		public string ChoiceId = "";

		protected override void OnStart() {
			base.OnStart();
		}

		protected override void OnStop() {
			base.OnStop();
		}

		protected override State OnUpdate() {
			if (string.IsNullOrEmpty(this.ChoiceId)) {
				PLogger.LogError("LastChoiceEqualsNode: choiceId is empty");
				return State.Failure;
			}

			return base.OnUpdate();
		}
	}
}
