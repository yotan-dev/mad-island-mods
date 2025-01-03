using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.AssWall;
using HarmonyLib;
using YotanModCore;

namespace Gallery.Patches
{
	public class AssWallPatch
	{
		private static AssWallTracker Tracker = null;

		[HarmonyPatch(typeof(SexManager), "AssWall")]
		[HarmonyPrefix]
		private static void Pre_SexManager_AssWall(int state, InventorySlot tmpWall)
		{
			if (Plugin.InGallery)
				return;

			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {};

				CommonStates player = null;
				CommonStates girl = null;

				if ((AssWallState) state == AssWallState.Start) {
					girl = Managers.mn?.inventory?.itemSlot?[50]?.common;
					charas.Add("girl", girl);

					player = Managers.mn?.gameMN?.playerCommons?[GameManager.selectPlayer];
					charas.Add("player", player);
				}

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "state", $"{state}" },
					{ "tmpWall", $"{tmpWall?.name ?? "null"}, type: {tmpWall?.type.ToString() ?? "null"}, size: {tmpWall?.size.ToString() ?? "null"}" },
				};
			
				GalleryLogger.SceneStart("AssWall", charas, infos);
			
				if ((AssWallState) state == AssWallState.Start && tmpWall != null) {
					Tracker = new AssWallTracker(player, girl, tmpWall.type);
					GalleryScenesManager.Instance.AddTrackerForCommon(player, Tracker);
					GalleryScenesManager.Instance.AddTrackerForCommon(girl, Tracker);
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("AssWall", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "AssWall")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_AssWall(IEnumerator result, int state, InventorySlot tmpWall)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			CommonStates player = null;
			CommonStates girl = null;
			try {
				Dictionary<string, CommonStates> charas = new Dictionary<string, CommonStates>() {};

				if ((AssWallState) state == AssWallState.Start) {
					girl = Managers.mn?.inventory?.itemSlot?[50]?.common;
					charas.Add("girl", girl);

					player = Managers.mn?.gameMN?.playerCommons?[GameManager.selectPlayer];
					charas.Add("player", player);
				}

				Dictionary<string, string> infos = new Dictionary<string, string>() {
					{ "state", $"{state}" },
					{ "tmpWall", $"{tmpWall?.name ?? "null"}, type: {tmpWall?.type.ToString() ?? "null"}, size: {tmpWall?.size.ToString() ?? "null"}" },
				};

				GalleryLogger.SceneEnd("AssWall", charas, infos);

				if ((AssWallState) state == AssWallState.Start && tmpWall != null) {
					Tracker?.End();
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("AssWall", error);
			} finally {
				if ((AssWallState) state == AssWallState.Start) {
					if (player != null)
						GalleryScenesManager.Instance.RemoveTrackerForCommon(player);
					if (girl != null)
						GalleryScenesManager.Instance.RemoveTrackerForCommon(girl);
					Tracker = null;
				}
			}
		}
	}
}
