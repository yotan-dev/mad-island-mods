using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.GalleryScenes.CommonSexPlayer;
using Gallery.GalleryScenes.PlayerRaped;
using HarmonyLib;

namespace Gallery.Patches
{
	/// <summary>
	/// Patch to integrate with Extended H-System mod and watch its scenes
	/// </summary>
	public class EHSWatcherPatch
	{
		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.CommonSexNPC), "Run")]
		[HarmonyPrefix]
		private static void Pre_CommonSexNPC_Run(ExtendedHSystem.Scenes.CommonSexNPC __instance)
		{
			__instance.AddEventHandler(
				new CommonSexNPCSceneEventHandler(__instance.NpcA, __instance.NpcB, __instance.Place, __instance.Type)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.CommonSexPlayer), "Run")]
		[HarmonyPrefix]
		private static void Pre_CommonSexPlayer_Run(ExtendedHSystem.Scenes.CommonSexPlayer __instance)
		{
			__instance.AddEventHandler(
				new CommonSexPlayerSceneEventHandler(__instance.Player, __instance.Npc, __instance.Type)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.PlayerRaped), "Run")]
		[HarmonyPrefix]
		private static void Pre_PlayerRaped_Run(ExtendedHSystem.Scenes.PlayerRaped __instance)
		{
			__instance.AddEventHandler(
				new PlayerRapedSceneEventHandler(__instance.Player, __instance.Rapist)
			);
		}
	}
}
