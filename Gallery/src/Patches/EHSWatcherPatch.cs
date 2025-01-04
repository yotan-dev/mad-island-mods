using Gallery.GalleryScenes.Daruma;
using HarmonyLib;

namespace Gallery.Patches
{
	/// <summary>
	/// Patch to integrate with Extended H-System mod and watch its scenes
	/// </summary>
	public class HFWatcherPatch
	{
		[HarmonyPatch(typeof(HFramework.Scenes.Daruma), "Run")]
		[HarmonyPrefix]
		private static void Pre_Daruma_Run(HFramework.Scenes.Daruma __instance)
		{
			__instance.AddEventHandler(
				new DarumaSceneEventHandler(__instance.Player, __instance.Npc)
			);
		}
	}
}
