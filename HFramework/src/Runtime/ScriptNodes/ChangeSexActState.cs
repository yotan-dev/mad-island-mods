using System;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Change SexState or ActType")]
	public class ChangeSexActState : Action
	{
		public enum TargetSexState
		{
			// Custom value to indicate no change
			DontChange = -1,

			// Copied from CommonStates.SexState
			None = 0,
			Playing = 1,
			GameOver = 2,
			Execution_Pole = 3,
			Daruma = 4
		}

		public enum TargetActType
		{
			// Custom value to indicate no change
			DontChange = -1,

			// Copied from NPCMove.ActType
			Idle = 0,
			Wait = 1,
			Travel = 2,
			RunAway = 3,
			Attack = 4,
			Damage = 5,
			Dead = 6,
			GoPoint = 7,
			Chase = 8,
			Interval = 9,
			Rapes = 10,
			AttackSP = 11,
			Follow = 12,
			Live = 13,
			Sleep = 14
		}

		[Serializable]
		public class ActorConfig
		{
			[ActorIndex]
			public int Index;
			public TargetSexState SexState = TargetSexState.DontChange;
			public TargetActType ActType = TargetActType.DontChange;
		}

		public ActorConfig[] Changes = new ActorConfig[0];

		protected override void OnStart() {
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			foreach (var actor in this.Changes) {
				var npc = this.context.Actors[actor.Index];
				if (actor.SexState != TargetSexState.DontChange)
					npc.Common.sex = (CommonStates.SexState)actor.SexState;

				if (actor.ActType != TargetActType.DontChange)
					npc.Common.GetComponent<NPCMove>().actType = (NPCMove.ActType)actor.ActType;
			}

			return State.Success;
		}
	}
}
