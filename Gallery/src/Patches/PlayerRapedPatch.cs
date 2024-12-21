using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Gallery.GalleryScenes.PlayerRaped;

namespace Gallery.Patches
{
	public class PlayerRapedPatch
	{
		private static PlayerRapedSceneEventHandler EventHandler;

		private static Dictionary<string, CommonStates> GetChars(CommonStates from, CommonStates to)
		{
			return new Dictionary<string, CommonStates>()
			{
				{"rapist", from},
				{"victim", to},
			};
		}

		private static Dictionary<string, string> GetInfos()
		{
			return new Dictionary<string, string>() { };
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPrefix]
		private static void Pre_SexManager_PlayerRaped(CommonStates from, CommonStates to)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("PlayerRaped", GetChars(from, to), GetInfos());
				
				EventHandler = new PlayerRapedSceneEventHandler(to, from);
				GalleryScenesManager.Instance.AddSceneHandlerForCommon(from, EventHandler);
				GalleryScenesManager.Instance.AddSceneHandlerForCommon(to, EventHandler);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("PlayerRaped", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_PlayerRaped(IEnumerator result, CommonStates from, CommonStates to)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("PlayerRaped", GetChars(from, to), GetInfos());

				EventHandler?.PlayerRaped(to, from, false);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("PlayerRaped", error);
			} finally {
				GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(from);
				GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(to);
			}
		}
	}
}