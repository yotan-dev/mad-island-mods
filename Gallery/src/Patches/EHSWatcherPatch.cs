using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.Slave;
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

		[HarmonyPatch(typeof(HFramework.Scenes.Slave), "Run")]
		[HarmonyPrefix]
		private static void Pre_Slave_Run(HFramework.Scenes.Slave __instance)
		{
			__instance.AddEventHandler(
				new SlaveSceneEventHandler(__instance.Player, __instance.TmpSlave)
			);
		}
	}
}
