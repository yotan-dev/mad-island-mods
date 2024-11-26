using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Gallery.GalleryScenes;

namespace Gallery.Patches
{
	public class CommonRapesNPCPatch
	{
		private static GalleryScenesManager GalleryManager { get { return GalleryScenesManager.Instance; } }

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "CommonRapesNPC")]
		[HarmonyPrefix]
		private static void Pre_SexManager_CommonRapesNPC(CommonStates npcA, CommonStates npcB, int sexType)
		{
			if (Plugin.InGallery)
				return;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {
					{"npcA", npcA},
					{"npcB", npcB},
				};

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "sexType", $"{sexType}" },
				};
			
				GalleryLogger.SceneStart("CommonRapesNPC", charas, infos, true);

				// GalleryManager.AddScene(new CommonRapesNPCScene(to, from));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("CommonRapesNPC", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "CommonRapesNPC")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_CommonRapesNPC(IEnumerator result, CommonStates npcA, CommonStates npcB, int sexType)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {
					{"npcA", npcA},
					{"npcB", npcB},
				};

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "sexType", $"{sexType}" },
				};

				GalleryLogger.SceneEnd("CommonRapesNPC", charas, infos, true);
				// GalleryManager.EndScene(typeof(CommonRapesNPCScene), from, to);

				// 89 x 15 + pregnant on patch 0.1.8
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("CommonRapesNPC", error);
			}
		}
	}
}