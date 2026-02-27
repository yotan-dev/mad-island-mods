#nullable enable

using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;
using System.Linq;
using HFramework.SexScripts;

namespace HFramework.Patches
{
	public class SexChecksPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.SexCheck))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_SexCheck(CommonStates from, CommonStates to, ref bool __result)
		{
			// @TODO: Probably a good idea to group Prefabs per type so we don't have to run through ALL scripts.
			if (from.npcID == CommonUtils.GetActivePlayer().npcID)
			{
				// CommonSexPlayer
				__result = BundleLoader.Loader.Prefabs.Any(p => p is CommonSexPlayerScript && p.Info.CanStart(from, to));
				if (!__result && Config.Instance.EnableLegacyScenes.Value)
					__result = SexChecker.CanFriendSex(CommonSexPlayer.Name, from, to);
			}
			else
			{
				// CommonSexNPC
				__result = BundleLoader.Loader.Prefabs.Any(p => p is CommonSexNPCScript && p.Info.CanStart(from, to));
				if (!__result && Config.Instance.EnableLegacyScenes.Value)
					__result = SexChecker.CanFriendSex(CommonSexNPC.Name, from, to);
			}

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
			string sceneName;
			if (to.npcID == activePlayer)
			{
				sceneName = PlayerRaped.Name;
			}
			else if (from.npcID == activePlayer)
			{
				if (to.nMove.actType == NPCMove.ActType.Sleep && to.anim.state.GetCurrent(0).Animation.Name == "A_sleep")
					sceneName = ManRapesSleep.Name;
				else
					sceneName = ManRapes.Name;
			}
			else
			{
				// Npc x Npc
				return true; // @TODO: We don't have this yet
			}

			return SexChecker.CanRape(sceneName, from, to);
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
