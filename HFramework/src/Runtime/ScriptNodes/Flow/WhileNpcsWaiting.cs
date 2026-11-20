using YotanModCore;

namespace HFramework.ScriptNodes.Flow
{
	/// <summary>
	/// Script continues while Npcs are in "Wait" actType and, optionally, not being controlled.
	/// If any npc is not waiting, the node fails.
	/// If children succeeds, the node succeeds
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Flow/While Npcs Waiting")]
	public class WhileNpcsWaiting : Passthrough
	{
		public bool FailIfControlled = true;

		protected override void OnStart() { }

		protected override void OnStop() { }

		protected override State OnUpdate() {
			foreach (var npc in this.Context.Actors) {
				if (npc.Common.nMove?.actType != NPCMove.ActType.Wait) {
					PLogger.LogDebug("WhileNpcsWaitingNode: Npc is not waiting");
					return State.Failure;
				}

				if (FailIfControlled && CommonUtils.IsControlled(npc.Common)) {
					PLogger.LogDebug("WhileNpcsWaitingNode: Npc is controlled");
					return State.Failure;
				}
			}

			return Child.Update();
		}
	}
}
