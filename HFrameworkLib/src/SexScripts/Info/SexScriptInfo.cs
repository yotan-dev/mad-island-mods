using System;
using System.Collections.Generic;
using System.Linq;
using HFramework.Tree;

namespace HFramework.SexScripts.Info
{
	[Serializable]
	public class SexScriptInfo
	{
		public SexNpcInfo[] Npcs;

		public bool NpcOrderMatters;

		public List<ConditionGroup> StartConditions;

		public List<ConditionGroup> ExecuteConditions;

		private bool TryMatchUnorderedNpcs(CommonStates[] npcs, out CommonStates[] matchedNpcs)
		{
			//@FIXME: This will fail likely fail if e.g.:
			// NPC[0] -> ID 15, Pregnant = any
			// NPC[1] -> ID 15, Pregnant = Ready to birth
			//
			// We receive [Npc{Ready To birth}, Npc{Not Pregnant}]
			// As it will first assign the ready to birth as NPC[0].
			matchedNpcs = new CommonStates[this.Npcs.Length];

			for (int i = 0; i < npcs.Length; i++)
			{
				bool foundMatch = false;
				for (int j = 0; j < this.Npcs.Length; j++)
				{
					if (matchedNpcs[j] == null && this.Npcs[j].Pass(npcs[i]))
					{
						matchedNpcs[j] = npcs[i];
						foundMatch = true;
						break;
					}
				}

				if (!foundMatch)
				{
					PLogger.LogDebug($"Failed to match NPC {i} - {npcs[i].npcID}");
					return false;
				}
			}

			PLogger.LogDebug("All NPCs matched successfully");

			return true;
		}

		private bool HasNeededNpcs(CommonStates[] npcs)
		{
			if (npcs.Length != this.Npcs.Length)
				return false;

			if (this.NpcOrderMatters)
			{
				for (int i = 0; i < this.Npcs.Length; i++)
				{
					if (!this.Npcs[i].Pass(npcs[i]))
						return false;
				}
			}
			else
			{
				return this.TryMatchUnorderedNpcs(npcs, out _);
			}

			return true;
		}

		public bool CanStart(params CommonStates[] npcs)
		{
			PLogger.LogDebug($"CanStart: {string.Join(", ", npcs.Select(n => n.npcID))}");
			if (!this.HasNeededNpcs(npcs)) {
				PLogger.LogDebug("HasNeededNpcs returned false");
				return false;
			}

			if (this.StartConditions.Count == 0)
				return true;

			var result = this.StartConditions.Any(g => g.Pass());
			PLogger.LogDebug($"Start conditions result: {result}");
			return result;
		}

		public bool CanExecute(SexInfo info)
		{
			if (this.ExecuteConditions.Count == 0)
				return true;

			var result = this.ExecuteConditions.Any(g => g.Pass(info));
			PLogger.LogDebug($"Execute conditions result: {result}");
			return result;
		}
	}
}
