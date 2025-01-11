using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.ManRapes;
using HarmonyLib;

namespace Gallery.Patches
{
	public class ManRapesPatch
	{
		private static Dictionary<string, CommonStates> Getcharas(CommonStates girl, CommonStates man) {
			return new Dictionary<string, CommonStates>() {
				{ "man (rapist)", man },
				{ "girl (victim)", girl },
			};
		}

		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPrefix]
		private static void Pre_SexManager_ManRapes(CommonStates girl, CommonStates man)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("ManRapes", Getcharas(girl, man), new Dictionary<string, string>());

				var tracker = new ManRapesTracker(man, girl);
				GalleryScenesManager.Instance.AddTrackerForCommon(man, tracker);
				GalleryScenesManager.Instance.AddTrackerForCommon(girl, tracker);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapes", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_ManRapes(IEnumerator result, CommonStates girl, CommonStates man)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("ManRapes", Getcharas(girl, man), new Dictionary<string, string>());

				var trackerA = GalleryScenesManager.Instance.GetTrackerForCommon(man);
				var trackerB = GalleryScenesManager.Instance.GetTrackerForCommon(girl);
				trackerA.End();
				if (trackerA != trackerB)
					trackerB.End();
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapes", error);
			} finally {
				GalleryScenesManager.Instance.RemoveTrackerForCommon(man);
				GalleryScenesManager.Instance.RemoveTrackerForCommon(girl);
			}
		}
	}
}
