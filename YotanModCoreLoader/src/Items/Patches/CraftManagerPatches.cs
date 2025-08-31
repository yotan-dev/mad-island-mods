using HarmonyLib;

namespace YotanModCore.Items.Patches
{
	public static class CraftManagerPatches
	{
		/// <summary>
		/// Patches CraftManager.Awake to load custom recipes.
		/// CraftManager is reinitialized whenever there is a scene change, so we need to reload it every time
		/// </summary>
		/// <param name="chest"></param>
		/// <param name="tmpData"></param>
		/// <returns></returns>
		[HarmonyPatch(typeof(CraftManager), "Awake")]
		[HarmonyPostfix]
		private static void Post_CraftManagerPatches_Awake(CraftManager __instance)
		{
			CraftDB.Instance?.LoadRecipes(__instance);
		}
	}
}
