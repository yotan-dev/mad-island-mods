using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.Delivery;
using HarmonyLib;

namespace Gallery.Patches
{
	public class DeliveryPatch
	{
		private static Dictionary<string, CommonStates> GetChars(CommonStates common) {
			return new Dictionary<string, CommonStates>() { { "common", common } };
		}

		private static Dictionary<string, string> GetInfos(WorkPlace tmpWorkPlace = null, SexPlace tmpSexPlace = null) {
			string workplaceStr = "null";
			if (tmpWorkPlace != null) {
				workplaceStr = $"{tmpWorkPlace.name ?? "null"}, workType: {tmpWorkPlace.workType}";
			}

			string sexPlaceStr = "null";
			if (tmpSexPlace != null) {
				sexPlaceStr = $"{tmpSexPlace.name ?? "null"}, grade: {tmpSexPlace.grade}, type: {tmpSexPlace.placeType}";
			}
			return new Dictionary<string, string>() {
				{ "tmpWorkPlace", workplaceStr },
				{ "tmpSexPlace", sexPlaceStr },
			};
		}

		[HarmonyPatch(typeof(SexManager), "Delivery")]
		[HarmonyPrefix]
		private static void Pre_SexManager_Delivery(CommonStates common, WorkPlace tmpWorkPlace = null, SexPlace tmpSexPlace = null)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("Delivery", GetChars(common), GetInfos(tmpWorkPlace, tmpSexPlace));

				var handler = new DeliverySceneEventHandler(common);
				GalleryScenesManager.Instance.AddSceneHandlerForCommon(common, handler);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("Delivery", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "Delivery")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_Delivery(IEnumerator result, CommonStates common, WorkPlace tmpWorkPlace = null, SexPlace tmpSexPlace = null)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("Delivery", GetChars(common), GetInfos(tmpWorkPlace, tmpSexPlace));

				var handler = GalleryScenesManager.Instance.GetSceneHandlerForCommon(common);
				if (handler != null && handler is DeliverySceneEventHandler deliveryHandler)
					deliveryHandler.AfterDelivery(null, common);
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("Delivery", error);
			} finally {
				GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(common);
			}
		}
	}
}