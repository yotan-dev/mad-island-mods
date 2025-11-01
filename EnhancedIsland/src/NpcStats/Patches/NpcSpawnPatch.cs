using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;

namespace EnhancedIsland.NpcStats.Patches
{
	public class NpcSpawnPatch
	{
		private static List<CommonStates> PendingTamedNpcs = [];

		[HarmonyPatch(typeof(RandomCharacter), nameof(RandomCharacter.GenChildSet))]
		[HarmonyPrefix]
		private static void Pre_RandChar_GenChildSet(CommonStates child)
		{
			StatDistributor.RedistributeStats(NpcKind.Newborn, child);
		}

		[HarmonyPatch(typeof(ItemManager), nameof(ItemManager.SummonNPC))]
		[HarmonyPostfix]
		private static IEnumerator Post_ItemManager_SummonNPC(IEnumerator result, int slotID)
		{
			ItemSlot tmpSlot = Managers.mn.inventory.itemSlot[slotID];
			CommonStates? common = null;
			bool wasWild = false;
			if (tmpSlot?.common != null)
			{
				if (PendingTamedNpcs.Count > 0)
					PLogger.LogWarning($"SummonNPC: There are {PendingTamedNpcs.Count} pending tamed NPCs. Expect 0");

				PendingTamedNpcs.Add(tmpSlot.common);
				common = tmpSlot.common;
				wasWild = common.friendID < 10;
			}

			while (result.MoveNext())
				yield return result.Current;

			if (common != null)
			{
				PendingTamedNpcs.Remove(common);
				if (wasWild && common.friendID >= 10)
					StatDistributor.RedistributeStats(NpcKind.Tamed, common);
			}
		}
	}
}
