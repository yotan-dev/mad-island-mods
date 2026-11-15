using HarmonyLib;
using YoUnnoficialPatches.Patches;

namespace YoUnnoficialPatches.Patches.Compat
{
	/// <summary>
	/// v0.6.0+: StoryManager.BossBattleCheck takes CommonStates[] (multiple bosses).
	/// </summary>
	public static class BossBattleCheckPatch
	{
		[HarmonyPatch(typeof(StoryManager), "BossBattleCheck")]
		[HarmonyPrefix]
		private static void Pre_StoryManager_BossBattleCheck(CommonStates[] bossCommon, bool teleport)
		{
			if (bossCommon == null)
				return;

			foreach (var boss in bossCommon)
				TranslationPatch.FixBossName(boss);
		}
	}
}
