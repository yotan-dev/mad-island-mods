using System.Collections;
using HarmonyLib;

namespace Gallery.Patches.CommonSexPlayer
{
	public class CommonSexPlayerPatchV1_006 : CommonSexPlayerBasePatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPrefix]
		private static void Pre_SexManager_CommonSexPlayer_(CommonStates pCommon, CommonStates nCommon, int sexType, int state, SexManager __instance)
		{
			CommonSexPlayerBasePatch.Pre_SexManager_CommonSexPlayer(pCommon, nCommon, sexType, state, __instance);
		}

		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_CommonSexPlayer_(IEnumerator result, CommonStates pCommon, CommonStates nCommon, int state, int sexType)
		{
			yield return CommonSexPlayerBasePatch.Post_SexManager_CommonSexPlayer(result, pCommon, nCommon, state, sexType);
		}
	}
}