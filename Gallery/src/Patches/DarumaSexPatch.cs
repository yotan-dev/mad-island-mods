using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;

namespace Gallery.Patches
{
	public class DarumaSexPatch
	{
		public struct DarumaInfo
		{
			public CommonStates Player;
			
			public CommonStates Girl;

			public DarumaInfo(CommonStates player, CommonStates girl) {
				this.Player = player;
				this.Girl = girl;
			}
		}

		public delegate void OnSceneInfo(DarumaInfo info);

		public static event OnSceneInfo OnStart;
		
		public static event OnSceneInfo OnBust;
		
		public static event OnSceneInfo OnEnd;

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

				// Place doesn't matter (v0.12)
				if (state == DarumaSexState.MainLoop) {
					OnStart?.Invoke(new DarumaInfo(Managers.mn.gameMN.playerCommons[1], Managers.mn.inventory.itemSlot[50].common));
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
					OnBust?.Invoke(new DarumaInfo(Managers.mn.gameMN.playerCommons[1], Managers.mn.inventory.itemSlot[50].common));
				}
				if (state == DarumaSexState.MainLoop) {
					OnEnd?.Invoke(new DarumaInfo(Managers.mn.gameMN.playerCommons[1], Managers.mn.inventory.itemSlot[50].common));
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("DarumaSex", error);
			}
		}
	}
}