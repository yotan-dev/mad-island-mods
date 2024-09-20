using System.Collections;
using HarmonyLib;

namespace Gallery.Patches.CommonSexPlayer
{
	public class CommonSexPlayerPatchV0_012 : CommonSexPlayerBasePatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPrefix]
		private static void Pre_SexManager_CommonSexPlayer_(CommonStates girl, CommonStates man, int sexType, int state, SexManager __instance)
		{
			CommonSexPlayerBasePatch.Pre_SexManager_CommonSexPlayer(girl, man, sexType, state, __instance);
		}

		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_CommonSexPlayer_(IEnumerator result, CommonStates girl, CommonStates man, int state, int sexType)
		{
			yield return CommonSexPlayerBasePatch.Post_SexManager_CommonSexPlayer(result, girl, man, state, sexType);
		}
	}
}