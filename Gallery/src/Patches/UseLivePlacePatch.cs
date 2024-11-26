using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Gallery.GalleryScenes;
using UnityEngine;

namespace Gallery.Patches
{
	public class UseLivePlacePatch
	{
		private static GalleryScenesManager GalleryManager { get { return GalleryScenesManager.Instance; } }

		private static Dictionary<string, CommonStates> GetChars() { return new Dictionary<string, CommonStates>() { }; }

		private static Dictionary<string, string> GetInfos(int state, ItemInfo tmpInfo) {
			var tmpInfoStr = "null";
			if (tmpInfo != null) {
				tmpInfoStr = $"ItemKey: {tmpInfo?.itemKey ?? "null"}";
			}

			return new Dictionary<string, string>() {
				{ "state", $"{state}" },
				{ "tmpInfo", tmpInfoStr },
			};
		}


		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "UseLivePlace")]
		[HarmonyPrefix]
		private static void Pre_SexManager_UseLivePlace(int state, ItemInfo tmpInfo = null)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("UseLivePlace", GetChars(), GetInfos(state, tmpInfo), true);

				// GalleryManager.AddScene(new UseLivePlaceScene(to, from));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("UseLivePlace", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "UseLivePlace")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_UseLivePlace(IEnumerator result, int state, ItemInfo tmpInfo = null)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("UseLivePlace", GetChars(), GetInfos(state, tmpInfo), true);
				// GalleryManager.EndScene(typeof(UseLivePlaceScene), from, to);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("UseLivePlace", error);
			}
		}
	}
}