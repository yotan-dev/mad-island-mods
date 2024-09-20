using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Gallery.GalleryScenes;

namespace Gallery.Patches
{
	public class PlaySexPatch
	{
		private static GalleryScenesManager GalleryManager { get { return GalleryScenesManager.Instance; } }

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "PlaySex")]
		[HarmonyPrefix]
		private static void Pre_SexManager_PlaySex(CommonStates to, CommonStates from, bool grapple = false)
		{
			if (Plugin.InGallery)
				return;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {
					{"from", from},
					{"to", to},
				};

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "grapple", $"{grapple}" },
				};
			
				GalleryLogger.SceneStart("PlaySex", charas, infos, true);

				// GalleryManager.AddScene(new PlaySexScene(to, from));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("PlaySex", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "PlaySex")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_PlaySex(IEnumerator result, CommonStates to, CommonStates from, bool grapple = false)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {
					{"from", from},
					{"to", to},
				};

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "grapple", $"{grapple}" },
				};

				GalleryLogger.SceneEnd("PlaySex", charas, infos, true);
				// GalleryManager.EndScene(typeof(PlaySexScene), from, to);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("PlaySex", error);
			}
		}
	}
}