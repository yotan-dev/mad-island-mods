using System.Collections;
using Gallery.Handlers;
using HarmonyLib;
using HFramework.Scenes;
using YotanModCore;

namespace Gallery.Patches
{
	/// <summary>
	/// Patches to make Gallery work fine.
	/// Usually patching parts of HFramework that are not pluggable for Gallery needs.
	/// </summary>
	public class GalleryScenePatch
	{
		[HarmonyPatch(typeof(ManRapes), "PerformGrapple")]
		[HarmonyPrefix]
		private static bool Pre_ManRapes_PerformGrapple(ref IEnumerator __result, ManRapes __instance, CommonStates ___Girl)
		{
			if (!Plugin.InGallery)
				return true;

			__result = new GalleryPlayerGrapples(__instance, ___Girl).Handle();
			return false;
		}

		[HarmonyPatch(typeof(PlayerRaped), "PerformGrapple")]
		[HarmonyPrefix]
		private static bool Pre_PlayerRaped_PerformGrapple(ref IEnumerator __result, ManRapes __instance, CommonStates ___Player)
		{
			if (!Plugin.InGallery)
				return true;

			__result = new GalleryPlayerGrappled(__instance, ___Player).Handle();
			return false;
		}

		private static IEnumerator Dummy()
		{
			yield break;
		}

		[HarmonyPatch(typeof(PlayerRaped), "StartRespawn")]
		[HarmonyPrefix]
		private static bool Pre_PlayerRaped_StartRespawn(ref IEnumerator __result)
		{
			if (!Plugin.InGallery)
				return true;

			__result = Dummy();
			return false;
		}

		[HarmonyPatch(typeof(PlayerRaped), "Respawn")]
		[HarmonyPrefix]
		private static bool Pre_PlayerRaped_Respawn()
		{
			if (!Plugin.InGallery)
				return true;

			return false;
		}

		[HarmonyPatch(typeof(GameManager), "MapIDCheck")]
		[HarmonyPrefix]
		public static bool Pre_GameManager_MapIDCheck(ref int __result)
		{
			if (!Plugin.InGallery)
				return true;

			/**
			 * MapGen is null in the gallery, and when setting up Daruma,
			 * the game calls NPCTeleport which ends up calling MapIDCheck,
			 * so we override it to return -1, which is a reserved value for no map
			 * in the reset of the logic.
			 */
			if (Managers.mn.gameMN.mapGen == null)
			{
				__result = -1;
				return false;
			}

			return true;
		}
	}
}
