using System;
using YotanModCore;
using YotanModCore.Consts;


namespace HFramework.SexScripts.Info
{
	[Serializable]
	public class SexNpcInfo
	{
		public int NpcID;
		public PregnantState Pregnant;
		public FaintState Faint;
		public DeadState Dead;

		private bool CheckPregnant(CommonStates common)
		{
			PLogger.LogDebug($"Checking pregnant state for NPC {common.npcID}. To match: {this.Pregnant}");
			switch (this.Pregnant) {
				case PregnantState.Any:
					return true;

				case PregnantState.NotPregnant:
					return !CommonUtils.IsPregnant(common);

				case PregnantState.Pregnant:
					return CommonUtils.IsPregnant(common);

				case PregnantState.NotReadyToGiveBirth:
					return !CommonUtils.IsPregnant(common) || common.pregnant[PregnantIndex.TimeToBirth] != 0;

				case PregnantState.PregNotReadyToGiveBirth:
					return CommonUtils.IsPregnant(common) && common.pregnant[PregnantIndex.TimeToBirth] != 0;

				case PregnantState.ReadyToGiveBirth:
					return CommonUtils.IsPregnant(common) && common.pregnant[PregnantIndex.TimeToBirth] == 0;
			}

			return false;
		}

		private bool CheckFaint(CommonStates common)
		{
			switch (this.Faint) {
				case FaintState.Any:
					return true;

				case FaintState.NotFainted:
					return common.faint > 0 && common.life > 0;

				// @TODO: HFramework conditionals used life too, but I don't remember why
				case FaintState.Fainted:
					return common.faint <= 0 || common.life <= 0;
			}

			return false;
		}

		private bool CheckDead(CommonStates common)
		{
			switch (this.Dead) {
				case DeadState.Any:
					return true;

				case DeadState.NotDead:
					return common.dead == 0;

				case DeadState.Dead:
					return common.dead != 0;
			}

			return false;
		}

		public bool Pass(CommonStates npc)
		{
			if (npc.npcID != this.NpcID) {
				PLogger.LogDebug($"NPC ID mismatch: {npc.npcID} != {this.NpcID}");
				return false;
			}

			if (!this.CheckPregnant(npc)) {
				PLogger.LogDebug($"Pregnant check failed for NPC {npc.npcID}");
				return false;
			}

			if (!this.CheckFaint(npc)) {
				PLogger.LogDebug($"Faint check failed for NPC {npc.npcID}");
				return false;
			}

			if (!this.CheckDead(npc)) {
				PLogger.LogDebug($"Dead check failed for NPC {npc.npcID}");
				return false;
			}

			PLogger.LogDebug($"NPC {npc.npcID} passed all checks");
			return true;
		}
	}
}
