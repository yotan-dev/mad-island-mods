using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Gallery.GalleryScenes;

namespace Gallery.Patches
{
	public class ManRapesSleepPatch
	{
		public struct ManSleepRapesInfo
		{
			public CommonStates man;
			public CommonStates girl;

			public ManSleepRapesInfo(CommonStates man, CommonStates girl) {
				this.man = man;
				this.girl = girl;
			}
		}

		public delegate void OnSceneInfo(ManSleepRapesInfo info);

		public delegate void SexTypeChangeInfo(int state, int sexType);

		public static event OnSceneInfo OnStart;

		public static event SexTypeChangeInfo OnSexTypeChange;
		
		public static event OnSceneInfo OnEnd;

		private static Dictionary<string, CommonStates> GetCharas(CommonStates girl, CommonStates man) {
			return new Dictionary<string, CommonStates>() {
				{ "man (rapist)", man },
				{ "girl (victim)", girl },
			};
		}

		private static Dictionary<string, string> GetInfos(int sexType, int state) {
			return new Dictionary<string, string>() {
				{ "state", $"{state}" },
				{ "sexType", $"{sexType}" },
			};
		}

		[HarmonyPatch(typeof(SexManager), "ManRapesSleep")]
		[HarmonyPrefix]
		private static void Pre_SexManager_ManRapesSleep(int state, CommonStates girl, CommonStates man, int sexType)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("ManRapesSleep", GetCharas(girl, man), GetInfos(sexType, state));

				switch ((ManRapesState) state) {
				case ManRapesState.Start: // Starting
					OnStart?.Invoke(new ManSleepRapesInfo(man, girl));
					break;

				case ManRapesState.StartRape: // Starting Rape
				case ManRapesState.StartDiscretlyRape: // Starting Discretly Rape
					OnSexTypeChange?.Invoke(state, sexType);
					GalleryLogger.LogDebug($"ManSleepRapeScene: OnStart: mode = {(ManRapesSexType) sexType}");
					break;

				case ManRapesState.Insert: // Inserting
				case ManRapesState.ChangeSpeed: // Changing speed
				case ManRapesState.Bust: // Busting
				case ManRapesState.Leave: // Leave
				case ManRapesState.SleepPowderSex: // Sleep Powder
					/* Nothing to do here. sexType is always 0 */
					break;

				default:
					GalleryLogger.LogError($"Unknown state: {state}");
					break;
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapesSleep", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "ManRapesSleep")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_ManRapesSleep(IEnumerator result, int state, CommonStates girl, CommonStates man, int sexType)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("ManRapesSleep", GetCharas(girl, man), GetInfos(sexType, state));
				if ((ManRapesState) state == ManRapesState.Start) {
					OnEnd?.Invoke(new ManSleepRapesInfo(man, girl));
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapesSleep", error);
			}
		}
	}
}