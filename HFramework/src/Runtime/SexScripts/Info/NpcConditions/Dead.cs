using System;

namespace HFramework.SexScripts.Info.NpcConditions
{
	/// <summary>
	/// Condition for checking if an NPC is dead or not.
	///
	/// This does not inherit from "NpcCondition" because it is a always present rule,
	/// and is not meant to be used as eventual condition like others.
	/// </summary>
	[Serializable]
	[Experimental]
	public class Dead : INpcCondition
	{
		public enum DeadState
		{
			Any,
			NotDead,
			Dead,
		}

		public DeadState Expected = DeadState.Any;

		public bool Pass(CommonStates common) {
			return this.Expected switch {
				DeadState.Any => true,
				DeadState.NotDead => common.dead == 0,
				DeadState.Dead => common.dead != 0,
				_ => false,
			};
		}
	}
}
