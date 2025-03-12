using HarmonyLib;

namespace EnhancedIsland.WarpBody
{
	internal class Main
	{
		internal void Init()
		{
			if (!PConfig.Instance.EnableWarpBody.Value)
				return;

			Harmony.CreateAndPatchAll(typeof(PlayerMovePatch));

			PLogger.LogInfo($"Warp Body is loaded!");
		}
	}
}
