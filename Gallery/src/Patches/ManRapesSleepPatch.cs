using System;
using System.Collections;
using System.Collections.Generic;
using HFramework.Scenes;
using Gallery.GalleryScenes.ManSleepRape;
using HarmonyLib;

namespace Gallery.Patches
{
	public class ManRapesSleepPatch
	{
		private static ManSleepRapeSceneEventHandler EventHandler;

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
					EventHandler = new ManSleepRapeSceneEventHandler(man, girl);
					GalleryScenesManager.Instance.AddSceneHandlerForCommon(man, EventHandler);
					GalleryScenesManager.Instance.AddSceneHandlerForCommon(girl, EventHandler);
					break;

				case ManRapesState.StartRape: // Starting Rape
					EventHandler?.OnSleepRapeTypeChange(null, ManRapeSleepState.ForcefullRape);
					GalleryLogger.LogDebug($"ManSleepRapeScene: OnStart: mode = {(ManRapeSleepState) sexType}");
					break;

				case ManRapesState.StartDiscretlyRape: // Starting Discretly Rape
					if (sexType == 0) {
						EventHandler?.OnSleepRapeTypeChange(null, ManRapeSleepState.SleepPowder);
					} else {
						EventHandler?.OnSleepRapeTypeChange(null, ManRapeSleepState.GentlyRape);
					}

					GalleryLogger.LogDebug($"ManSleepRapeScene: OnStart: mode = {(ManRapeSleepState) sexType}");
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
					EventHandler?.AfterManRape(girl, man);
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapesSleep", error);
			} finally {
				if ((ManRapesState) state == ManRapesState.Start) {
					GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(man);
					GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(girl);
					EventHandler = null;
				}
			}
		}
	}
}
