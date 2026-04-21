using YotanModCore;

namespace HFramework.ScriptNodes
{
	/// <summary>
	/// Marks a section that will be skipped if the player presses the skip button.
	/// It will Success as soon as skip is clicked, regardless of child node states.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Flow/Skippable Section")]
	public class SkippableSection : Passthrough
	{
		protected override void OnStart() {

		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			if (Managers.mn.uiMN.skip) {
				return State.Success;
			}

			return child.Update();
		}
	}
}
