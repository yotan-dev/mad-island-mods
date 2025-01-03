using Gallery.GalleryScenes.Daruma;
using Gallery.GalleryScenes.Delivery;
using Gallery.GalleryScenes.Onani;
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

		[HarmonyPatch(typeof(HFramework.Scenes.Delivery), "Run")]
		[HarmonyPrefix]
		private static void Pre_Delivery_Run(HFramework.Scenes.Delivery __instance)
		{
			__instance.AddEventHandler(
				new DeliverySceneEventHandler(__instance.Girl)
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
	}
}
