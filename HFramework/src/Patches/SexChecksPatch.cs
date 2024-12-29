#nullable enable

using HFramework.Performer;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;

namespace HFramework.Patches
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
				sceneInfo = ScenesManager.Instance.GetSceneInfo(CommonSexPlayer.Name);
			else
				sceneInfo = ScenesManager.Instance.GetSceneInfo(CommonSexNPC.Name);

			__result = sceneInfo?.CanStart(PerformerScope.Sex, realFrom, realTo) ?? false;
			return false;
		}
	}
}
