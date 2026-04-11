using System;

namespace HFramework.SexScripts.Info.NpcConditions
{
	/// <summary>
	/// Condition for checking if an NPC is fainted or not.
	///
	/// This does not inherit from "NpcCondition" because it is a always present rule,
	/// and is not meant to be used as eventual condition like others.
	/// </summary>
	[Serializable]
	[Experimental]
	public class Faint : INpcCondition
	{
		public enum FaintState
		{
			Any,
			NotFainted,
			Fainted,
		}

		public FaintState Expected = FaintState.Any;

		public bool Pass(CommonStates common) {
			return this.Expected switch {
				FaintState.Any => true,
				FaintState.NotFainted => common.faint > 0 && common.life > 0,

				// @TODO: HFramework conditionals used life too, but I don't remember why
				FaintState.Fainted => common.faint <= 0 || common.life <= 0,
				_ => false,
			};
		}
	}
}
