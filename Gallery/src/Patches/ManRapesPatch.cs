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

				var handler = new ManRapesSceneEventHandler(man, girl);
				GalleryScenesManager.Instance.AddSceneHandlerForCommon(man, handler);
				GalleryScenesManager.Instance.AddSceneHandlerForCommon(girl, handler);
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

				var handlerA = GalleryScenesManager.Instance.GetSceneHandlerForCommon(man);
				var handlerB = GalleryScenesManager.Instance.GetSceneHandlerForCommon(girl);
				handlerA.AfterManRape(girl, man);
				if (handlerA != handlerB)
					handlerB.AfterManRape(man, girl);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapes", error);
			} finally {
				GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(man);
				GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(girl);
			}
		}
	}
}
