using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.Daruma;
using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;

namespace Gallery.Patches
{
	public class DarumaSexPatch
	{
		private static DarumaSceneEventHandler EventHandler;

		[HarmonyPatch(typeof(SexManager), "DarumaSex")]
		[HarmonyPrefix]
		private static void Pre_SexManager_DarumaSex(int state, InventorySlot tmpDaruma = null)
		{
			if (Plugin.InGallery)
				return;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {};

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "state", $"{state}" },
					{ "tmpDaruma", $"{tmpDaruma?.name ?? "null"}, type: {tmpDaruma?.type.ToString() ?? "null"}, size: {tmpDaruma?.size.ToString() ?? "null"}" },
				};
			
				GalleryLogger.SceneStart("DarumaSex", charas, infos);

				// Place doesn't matter (v0.2.3)
				if (state == DarumaSexState.MainLoop) {
					var player = CommonUtils.GetActivePlayer();
					var girl = Managers.mn.inventory.itemSlot[50].common;
					EventHandler = new DarumaSceneEventHandler(player, girl);
					
					GalleryScenesManager.Instance.AddSceneHandlerForCommon(player, EventHandler);
					GalleryScenesManager.Instance.AddSceneHandlerForCommon(girl, EventHandler);
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("DarumaSex", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "DarumaSex")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_DarumaSex(IEnumerator result, int state, InventorySlot tmpDaruma = null)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {};

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "state", $"{state}" },
					{ "tmpDaruma", $"{tmpDaruma?.name ?? "null"}, type: {tmpDaruma?.type.ToString() ?? "null"}, size: {tmpDaruma?.size.ToString() ?? "null"}" },
				};

				GalleryLogger.SceneEnd("DarumaSex", charas, infos);
				
				if (state == DarumaSexState.Bust) {
					EventHandler.OnBusted(null, null, 0);
				}
				if (state == DarumaSexState.MainLoop) {
					EventHandler.AfterSex(null, null, null);
					GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(CommonUtils.GetActivePlayer());
					GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(Managers.mn.inventory.itemSlot[50].common);
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("DarumaSex", error);
			}
		}
	}
}