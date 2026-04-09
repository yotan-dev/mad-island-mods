using HFramework.Performer;
using HFramework.Scenes;
using YotanModCore;

namespace HFramework
{
	public static class SexChecker
	{
		public static bool CanFriendSex(string sceneName, CommonStates from, CommonStates to)
		{
			if (!Managers.mn.npcMN.IsActiveFriend(to))
			{
				PLogger.LogDebug($"CanFriendSex: {from.charaName} -> {to.charaName} is not a friend");
				return false;
			}

			var actors = Utils.SortActors(from, to);

			var realFrom = actors[0];
			var realTo = actors.Length >= 2 ? actors[1] : null;

			var sceneInfo = ScenesManager.Instance.GetSceneInfo(sceneName);
			PLogger.LogConditionDebug($"SexCheck({sceneInfo?.Name}): {from.charaName} -> {to.charaName}");
			bool result = sceneInfo?.CanStart(PerformerScope.Sex, realFrom, realTo) ?? false;
			PLogger.LogDebug($"SexCheck({sceneInfo?.Name}): {from.charaName} -> {to.charaName} = {result}");

			return result;
		}

		public static bool CanRape(string sceneName, CommonStates from, CommonStates to)
		{
			// NOTE: We don't sort actors for rape scenes, because we need to know WHO is raping and WHO is being raped.
			//       For example, LargeNativeFemale can rape Man (thus [LargeNativeFemale, Man]),
			//       but Man can rape LargeNativeFemale too (thus [Man, LargeNativeFemale]).
			//       In those cases, we will have additional performers to support that.

			var sceneInfo = ScenesManager.Instance.GetSceneInfo(sceneName);
			if (sceneInfo == null)
				return false;

			bool result = sceneInfo.CanStart(PerformerScope.Sex, from, to);
			PLogger.LogDebug($"RapesCheck({sceneInfo?.Name}): {from.charaName} -> {to.charaName} = {result}");

			return result;
		}
	}
}
