using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;

namespace Gallery.Patches
{
	public class AssWallPatch
	{
		public struct AssWallInfo
		{
			public CommonStates Player;
			
			public CommonStates Girl;

			public InventorySlot.Type WallType;

			public AssWallInfo(CommonStates player, CommonStates girl, InventorySlot.Type wallType) {
				this.Player = player;
				this.Girl = girl;
				this.WallType = wallType;
			}
		}

		public delegate void OnSceneInfo(AssWallInfo info);

		public static event OnSceneInfo OnStart;
		
		public static event OnSceneInfo OnEnd;

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
					OnStart?.Invoke(new AssWallInfo(player, girl, tmpWall.type));
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

				GalleryLogger.SceneEnd("AssWall", charas, infos);

				if ((AssWallState) state == AssWallState.Start && tmpWall != null) {
					OnEnd?.Invoke(new AssWallInfo(player, girl, tmpWall.type));
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("AssWall", error);
			}
		}
	}
}