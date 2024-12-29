using Gallery.GalleryScenes.AssWall;
using Gallery.GalleryScenes.CommonSexNPC;
using Gallery.GalleryScenes.CommonSexPlayer;
using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.Delivery;
using Gallery.GalleryScenes.ManRapes;
using Gallery.GalleryScenes.ManSleepRape;
using Gallery.GalleryScenes.Onani;
using Gallery.GalleryScenes.PlayerRaped;
using Gallery.GalleryScenes.Slave;
using Gallery.GalleryScenes.Toilet;
using HarmonyLib;
using YotanModCore;

namespace Gallery.Patches
{
	/// <summary>
	/// Patch to integrate with Extended H-System mod and watch its scenes
	/// </summary>
	public class HFWatcherPatch
	{
		[HarmonyPatch(typeof(HFramework.Scenes.AssWall), "Run")]
		[HarmonyPrefix]
		private static void Pre_AssWall_Run(HFramework.Scenes.AssWall __instance)
		{
			__instance.AddEventHandler(
				new AssWallSceneEventHandler(__instance.Player, __instance.Npc, __instance.TmpWall.type)
			);
		}

		[HarmonyPatch(typeof(HFramework.Scenes.Daruma), "Run")]
		[HarmonyPrefix]
		private static void Pre_Daruma_Run(HFramework.Scenes.Daruma __instance)
		{
			__instance.AddEventHandler(
				new DarumaSceneEventHandler(__instance.Player, __instance.Npc)
			);
		}

		[HarmonyPatch(typeof(HFramework.Scenes.Delivery), "Run")]
		[HarmonyPrefix]
		private static void Pre_Delivery_Run(HFramework.Scenes.Delivery __instance)
		{
			__instance.AddEventHandler(
				new DeliverySceneEventHandler(__instance.Girl)
			);
		}

		[HarmonyPatch(typeof(HFramework.Scenes.ManRapes), "Run")]
		[HarmonyPrefix]
		private static void Pre_ManRapes_Run(HFramework.Scenes.ManRapes __instance)
		{
			__instance.AddEventHandler(
				new ManRapesSceneEventHandler(__instance.Man, __instance.Girl)
			);
		}

		[HarmonyPatch(typeof(HFramework.Scenes.OnaniNPC), nameof(HFramework.Scenes.OnaniNPC.Run))]
		[HarmonyPrefix]
		private static void Pre_OnaniNPC_Run(HFramework.Scenes.OnaniNPC __instance)
		{
			__instance.AddEventHandler(
				new OnaniSceneEventHandler(__instance.Npc)
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

		[HarmonyPatch(typeof(HFramework.Scenes.Toilet), nameof(HFramework.Scenes.Toilet.Run))]
		[HarmonyPrefix]
		private static void Pre_Toilet_Run(HFramework.Scenes.Toilet __instance)
		{
			__instance.AddEventHandler(
				new ToiletSceneEventHandler(__instance.Player, __instance.Npc)
			);
		}

		[HarmonyPatch(typeof(HFramework.Scenes.PlayerRaped), "Run")]
		[HarmonyPrefix]
		private static void Pre_PlayerRaped_Run(HFramework.Scenes.PlayerRaped __instance)
		{
			__instance.AddEventHandler(
				new PlayerRapedSceneEventHandler(__instance.Player, __instance.Rapist)
			);
		}
	}
}
