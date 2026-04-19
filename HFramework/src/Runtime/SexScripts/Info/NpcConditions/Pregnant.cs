using System;
using HFramework;
using HFramework.SexScripts.Info.NpcConditions;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

[Serializable]
[Experimental]
public class Pregnant : NpcCondition
{
	[Flags]
	public enum PregnancyStatus : int {
		[InspectorName("None - (DO NOT USE) Will never pass")]
		None = 0,

		/// <summary>
		/// Not pregnant at all
		/// </summary>
		NotPregnant = 1 << 0,

		/// <summary>
		/// Pregnant but not ready to give birth (no belly yet)
		/// </summary>
		PregnantNoBelly = 1 << 1,

		/// <summary>
		/// Pregnant and ready to give birth (has belly) - same as IsDueDate()
		/// </summary>
		PregnantReady = 1 << 2,

		[InspectorName("All - (DO NOT USE) Will always pass, just delete the condition")]
		All = ~0,
	}

	[Tooltip("Pregnancy states that are accepted (treated as 'OR')")]
	public PregnancyStatus Pregnancy = PregnancyStatus.NotPregnant;

	public override bool Pass(CommonStates common) {
		if (!CommonUtils.IsPregnant(common)) {
			return this.Pregnancy.HasFlag(PregnancyStatus.NotPregnant);
		}

		var isReadyToGiveBirth = common.pregnant[PregnantIndex.TimeToBirth] != 0;
		if (isReadyToGiveBirth) {
			return this.Pregnancy.HasFlag(PregnancyStatus.PregnantReady);
		} else {
			return this.Pregnancy.HasFlag(PregnancyStatus.PregnantNoBelly);
		}
	}
}
