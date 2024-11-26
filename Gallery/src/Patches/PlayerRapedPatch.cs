using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Gallery.GalleryScenes;

namespace Gallery.Patches
{
	public class PlayerRapedPatch
	{
		public struct PlayerRapedInfo
		{
			public CommonStates player;
			public CommonStates rapist;

			public PlayerRapedInfo(CommonStates player, CommonStates rapist) {
				this.player = player;
				this.rapist = rapist;
			}
		}

		public delegate void OnSceneInfo(PlayerRapedInfo info);

		public static event OnSceneInfo OnStart;

		public static event OnSceneInfo OnEnd;

		private static Dictionary<string, CommonStates> GetChars(CommonStates from, CommonStates to)
		{
			return new Dictionary<string, CommonStates>()
			{
				{"rapist", from},
				{"victim", to},
			};
		}

		private static Dictionary<string, string> GetInfos()
		{
			return new Dictionary<string, string>() { };
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPrefix]
		private static void Pre_SexManager_PlayerRaped(CommonStates from, CommonStates to)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("PlayerRaped", GetChars(from, to), GetInfos());

				OnStart?.Invoke(new PlayerRapedInfo(to, from));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("PlayerRaped", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "PlayerRaped")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_PlayerRaped(IEnumerator result, CommonStates from, CommonStates to)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("PlayerRaped", GetChars(from, to), GetInfos());
				OnEnd?.Invoke(new PlayerRapedInfo(to, from));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("PlayerRaped", error);
			}
		}
	}
}