using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;
using Gallery.GalleryScenes;

namespace Gallery.Patches.CommonSexPlayer
{
	public class CommonSexPlayerBasePatch
	{
		public struct CommonSexPlayerInfo
		{
			public CommonStates Player;
			
			public CommonStates Npc;

			public int SexType;

			public CommonSexPlayerInfo(CommonStates player, CommonStates npc, int sexType) {
				this.Player = player;
				this.Npc = npc;
				this.SexType = sexType;
			}
		}

		public delegate void OnSceneInfo(CommonSexPlayerInfo info);

		public delegate void BustedInfo(int specialFlag);

		public static event OnSceneInfo OnStart;

		public static event BustedInfo OnBusted;
		
		public static event OnSceneInfo OnEnd;

		private static Dictionary<string, CommonStates> GetChars(CommonStates pCommon, CommonStates nCommon)
		{
			return new Dictionary<string, CommonStates>() {
				{"pCommon", pCommon},
				{"nCommon", nCommon},
			};
		}

		private static Dictionary<string, string> GetInfos(int sexType, int state)
		{
			return new Dictionary<string, string>() {
				{"state", $"{state}"},
				{"sexType", $"{sexType}"},
			};
		}

		protected static void Pre_SexManager_CommonSexPlayer(CommonStates pCommon, CommonStates nCommon, int sexType, int state, SexManager __instance)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("CommonSexPlayer", GetChars(pCommon, nCommon), GetInfos(sexType, state));

				switch ((CommonSexPlayerState)state) {
					case CommonSexPlayerState.Start:
						OnStart?.Invoke(new CommonSexPlayerInfo(pCommon, nCommon, sexType));
						break;

					case CommonSexPlayerState.Bust:
						OnBusted?.Invoke(__instance.tmpSexCountType);
						break;

					case CommonSexPlayerState.Caress:
					case CommonSexPlayerState.Insert:
					case CommonSexPlayerState.Speed:
					case CommonSexPlayerState.Leave:
					case CommonSexPlayerState.Hold:
					case CommonSexPlayerState.SwitchPosition:
						/* Nothing to do here */
						break;
				
					default:
						GalleryLogger.LogError($"CommonSexPlayer: Unknown state: {state}");
						break;
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("CommonSexPlayer", error);
			}
		}

		protected static IEnumerator Post_SexManager_CommonSexPlayer(IEnumerator result, CommonStates pCommon, CommonStates nCommon, int state, int sexType)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("CommonSexPlayer", GetChars(pCommon, nCommon), GetInfos(sexType, state));

				switch ((CommonSexPlayerState)state) {
					case CommonSexPlayerState.Start:
						OnEnd?.Invoke(new CommonSexPlayerInfo(pCommon, nCommon, sexType));
						break;

					case CommonSexPlayerState.Caress:
					case CommonSexPlayerState.Insert:
					case CommonSexPlayerState.Speed:
					case CommonSexPlayerState.Bust:
					case CommonSexPlayerState.Leave:
					case CommonSexPlayerState.Hold:
					case CommonSexPlayerState.SwitchPosition:
						/* Nothing to do here */
						break;
				
					default:
						GalleryLogger.LogError($"CommonSexPlayer: Unknown state: {state}");
						break;
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("CommonSexPlayer", error);
			}
		}
	}
}