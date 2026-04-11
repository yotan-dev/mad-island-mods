using System;
using HFramework.SexScripts.Info.NpcConditions;
using UnityEngine;


namespace HFramework.SexScripts.Info
{
	[Serializable]
	[Experimental]
	public class SexNpcInfo
	{
		public int NpcID;

		public Faint FaintCondition;

		public Dead DeadCondition;

		[SerializeReference, Subclass]
		public NpcCondition[] Conditions;

		public bool Pass(CommonStates npc)
		{
			if (npc.npcID != this.NpcID) {
				PLogger.LogDebug($"NPC ID mismatch: {npc.npcID} != {this.NpcID}");
				return false;
			}

			if (!this.FaintCondition.Pass(npc)) {
				PLogger.LogDebug($"Faint check failed for NPC {npc.npcID}");
				return false;
			}

			if (!this.DeadCondition.Pass(npc)) {
				PLogger.LogDebug($"Dead check failed for NPC {npc.npcID}");
				return false;
			}

			foreach (var condition in this.Conditions) {
				if (!condition.Pass(npc)) {
					PLogger.LogDebug($"Condition {condition.GetType().Name} failed for NPC {npc.npcID}");
					return false;
				}
			}

			PLogger.LogDebug($"NPC {npc.npcID} passed all checks");
			return true;
		}
	}
}
