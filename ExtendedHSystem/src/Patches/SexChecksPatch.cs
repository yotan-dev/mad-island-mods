#nullable enable

using System.Collections.Generic;
using ExtendedHSystem.Performer;
using ExtendedHSystem.Scenes;
using HarmonyLib;
using YotanModCore;

namespace ExtendedHSystem.Patches
{
	public class SexChecksPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.SexCheck))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_SexCheck(CommonStates from, CommonStates to, ref bool __result)
		{
			__result = false;
			
			if (!Managers.mn.npcMN.IsActiveFriend(to))
				return false;

			var actors = Utils.SortActors(from, to);

			var realFrom = actors[0];
			var realTo = actors.Length >= 2 ? actors[1] : null;

			SceneInfo? sceneInfo;
			if (from.npcID == CommonUtils.GetActivePlayer().npcID)
				sceneInfo = ScenesLoader.SceneInfos.GetValueOrDefault(CommonSexPlayer.Name, null);
			else
				sceneInfo = ScenesLoader.SceneInfos.GetValueOrDefault(CommonSexNPC.Name, null);

			__result = sceneInfo?.CanStart(PerformerScope.Sex, realFrom, realTo) ?? false;
			return false;
		}
	}
}
