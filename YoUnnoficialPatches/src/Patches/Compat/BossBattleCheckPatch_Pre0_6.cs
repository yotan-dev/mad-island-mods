using HarmonyLib;
using YoUnnoficialPatches.Patches;

namespace YoUnnoficialPatches.Patches.Compat
{
	/// <summary>
	/// < v0.6.0: StoryManager.BossBattleCheck takes a single CommonStates.
	/// </summary>
	public static class BossBattleCheckPatch_Pre0_6
	{
		[HarmonyPatch(typeof(StoryManager), "BossBattleCheck")]
		[HarmonyPrefix]
		private static void Pre_StoryManager_BossBattleCheck(CommonStates bossCommon, bool teleport)
		{
			TranslationPatch.FixBossName(bossCommon);
		}
	}
}
