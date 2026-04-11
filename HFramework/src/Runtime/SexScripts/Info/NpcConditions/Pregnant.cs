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
	[Header("Pregnancy Status\n(select all that applies -- treated as 'OR')")]
	/// <summary>
	/// If true, the NPC must not be pregnant.
	/// </summary>
	[Tooltip("If true, the NPC must not be pregnant.")]
	public bool NotPregnant;

	/// <summary>
	/// If true, the NPC must be pregnant but not ready to give birth.
	/// This means they are running the pregnancy cycle, but does not have a belly yet
	/// </summary>
	[Tooltip("If true, the NPC must be pregnant but not ready to give birth (no belly yet).")]
	public bool PregnantNoBelly;

	/// <summary>
	/// If true, the NPC must be ready to give birth.
	/// This means they have a belly and are ready to give birth.
	/// </summary>
	[Tooltip("If true, the NPC must be ready to give birth (has belly and is ready to give birth).")]
	public bool PregnantReady;

	public override bool Pass(CommonStates common) {
		if (!CommonUtils.IsPregnant(common)) {
			return this.NotPregnant;
		}

		var isReadyToGiveBirth = common.pregnant[PregnantIndex.TimeToBirth] != 0;
		if (isReadyToGiveBirth) {
			return this.PregnantReady;
		} else {
			return this.PregnantNoBelly;
		}
	}
}
