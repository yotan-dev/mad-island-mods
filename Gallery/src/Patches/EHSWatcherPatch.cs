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
	public class EHSWatcherPatch
	{
		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.AssWall), "Run")]
		[HarmonyPrefix]
		private static void Pre_AssWall_Run(ExtendedHSystem.Scenes.AssWall __instance)
		{
			__instance.AddEventHandler(
				new AssWallSceneEventHandler(__instance.Player, __instance.Npc, __instance.TmpWall.type)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.CommonSexNPC), "Run")]
		[HarmonyPrefix]
		private static void Pre_CommonSexNPC_Run(ExtendedHSystem.Scenes.CommonSexNPC __instance)
		{
			// We only track if at least one is friend, as we can get some weird results otherwise -- specially with herb village
			if (CommonUtils.IsFriend(__instance.NpcA) || CommonUtils.IsFriend(__instance.NpcB)) {
				__instance.AddEventHandler(
					new CommonSexNPCSceneEventHandler(__instance.NpcA, __instance.NpcB, __instance.Place, __instance.Type)
				);
			}
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.Daruma), "Run")]
		[HarmonyPrefix]
		private static void Pre_Daruma_Run(ExtendedHSystem.Scenes.Daruma __instance)
		{
			__instance.AddEventHandler(
				new DarumaSceneEventHandler(__instance.Player, __instance.Npc)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.Delivery), "Run")]
		[HarmonyPrefix]
		private static void Pre_Delivery_Run(ExtendedHSystem.Scenes.Delivery __instance)
		{
			__instance.AddEventHandler(
				new DeliverySceneEventHandler(__instance.Girl)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.ManRapes), "Run")]
		[HarmonyPrefix]
		private static void Pre_ManRapes_Run(ExtendedHSystem.Scenes.ManRapes __instance)
		{
			__instance.AddEventHandler(
				new ManRapesSceneEventHandler(__instance.Man, __instance.Girl)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.ManRapesSleep), "Run")]
		[HarmonyPrefix]
		private static void Pre_ManRapesSleep_Run(ExtendedHSystem.Scenes.ManRapesSleep __instance)
		{
			__instance.AddEventHandler(
				new ManSleepRapeSceneEventHandler(__instance.Man, __instance.Girl)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.OnaniNPC), nameof(ExtendedHSystem.Scenes.OnaniNPC.Run))]
		[HarmonyPrefix]
		private static void Pre_OnaniNPC_Run(ExtendedHSystem.Scenes.OnaniNPC __instance)
		{
			__instance.AddEventHandler(
				new OnaniSceneEventHandler(__instance.Npc)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.Slave), "Run")]
		[HarmonyPrefix]
		private static void Pre_Slave_Run(ExtendedHSystem.Scenes.Slave __instance)
		{
			__instance.AddEventHandler(
				new SlaveSceneEventHandler(__instance.Player, __instance.TmpSlave)
			);
		}

		[HarmonyPatch(typeof(ExtendedHSystem.Scenes.Toilet), nameof(ExtendedHSystem.Scenes.Toilet.Run))]
		[HarmonyPrefix]
		private static void Pre_Toilet_Run(ExtendedHSystem.Scenes.Toilet __instance)
		{
			__instance.AddEventHandler(
				new ToiletSceneEventHandler(__instance.Player, __instance.Npc)
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
