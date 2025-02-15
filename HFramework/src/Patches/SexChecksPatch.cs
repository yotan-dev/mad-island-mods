#nullable enable

using HFramework.Performer;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;

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
			{
				PLogger.LogDebug($"SexCheck: {from.charaName} -> {to.charaName} is not a friend");
				return false;
			}

			var actors = Utils.SortActors(from, to);

			var realFrom = actors[0];
			var realTo = actors.Length >= 2 ? actors[1] : null;

			SceneInfo? sceneInfo;
			if (from.npcID == CommonUtils.GetActivePlayer().npcID)
				sceneInfo = ScenesManager.Instance.GetSceneInfo(CommonSexPlayer.Name);
			else
				sceneInfo = ScenesManager.Instance.GetSceneInfo(CommonSexNPC.Name);

			__result = sceneInfo?.CanStart(PerformerScope.Sex, realFrom, realTo) ?? false;
			PLogger.LogDebug($"SexCheck({sceneInfo?.Name}): {from.charaName} -> {to.charaName} = {__result}");

			return false;
		}

		[HarmonyPatch(typeof(SexManager), nameof(SexManager.RapesCheck))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_RapesCheck(CommonStates from, CommonStates to, ref bool __result)
		{
			__result = false;

			// NOTE: We don't sort actors for rape scenes, because we need to know WHO is raping and WHO is being raped.
			//       For example, LargeNativeFemale can rape Man (thus [LargeNativeFemale, Man]),
			//       but Man can rape LargeNativeFemale too (thus [Man, LargeNativeFemale]).
			//       In those cases, we will have additional performers to support that.

			int activePlayer = CommonUtils.GetActivePlayer().npcID;
			SceneInfo? sceneInfo;
			if (to.npcID == activePlayer)
			{
				sceneInfo = ScenesManager.Instance.GetSceneInfo(PlayerRaped.Name);
			}
			else if (from.npcID == activePlayer)
			{
				if (to.nMove.actType == NPCMove.ActType.Sleep && to.anim.state.GetCurrent(0).Animation.Name == "A_sleep")
					sceneInfo = ScenesManager.Instance.GetSceneInfo(ManRapesSleep.Name);
				else
					sceneInfo = ScenesManager.Instance.GetSceneInfo(ManRapes.Name);
			}
			else
			{
				// Npc x Npc
				return true; // @TODO: We don't have this yet
			}

			if (sceneInfo != null)
			{
				__result = sceneInfo.CanStart(PerformerScope.Sex, from, to);
				return false;
			}

			return true;
		}

		[HarmonyPatch(typeof(NPCManager), nameof(NPCManager.LoveLoveCheck))]
		[HarmonyPostfix]
		private static bool Post_NPCManager_LoveLoveCheck(bool result, CommonStates commonA, CommonStates commonB)
		{
			// This is a check where we want to know if commonB accepts sex from commonA

			var sexTo = commonB;
			var sexFrom = commonA;

			// Player characters doesn't track love by default, so swap them so we can get the love of the NPC only.
			// @TODO: Can we improve that?
			if (sexTo.npcID == NpcID.Yona || sexTo.npcID == NpcID.Man)
			{
				var temp = sexTo;
				sexTo = sexFrom;
				sexFrom = temp;
			}

			int loversID = Managers.mn.npcMN.GetLoversID(sexTo, sexFrom.friendID);
			float loveVal = -1;
			if (loversID != -1)
				loveVal = sexTo.lovers[loversID].love;

			return loveVal >= 50f;
		}
	}
}
