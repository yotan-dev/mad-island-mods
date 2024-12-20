using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.CommonSexPlayer;

namespace Gallery.Patches.CommonSexPlayer
{
	public class CommonSexPlayerBasePatch
	{
		private static CommonSexPlayerSceneEventHandler EventHandler = null;

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
			try
			{
				GalleryLogger.SceneStart("CommonSexPlayer", GetChars(pCommon, nCommon), GetInfos(sexType, state));

				switch ((CommonSexPlayerState)state)
				{
					case CommonSexPlayerState.Start:
						EventHandler = new CommonSexPlayerSceneEventHandler(pCommon, nCommon, sexType);
						GalleryScenesManager.Instance.AddSceneHandlerForCommon(pCommon, EventHandler);
						GalleryScenesManager.Instance.AddSceneHandlerForCommon(nCommon, EventHandler);
						break;

					case CommonSexPlayerState.Bust:
						EventHandler?.OnBusted(null, null, __instance.tmpSexCountType);
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
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("CommonSexPlayer", error);
			}
		}

		protected static IEnumerator Post_SexManager_CommonSexPlayer(IEnumerator result, CommonStates pCommon, CommonStates nCommon, int state, int sexType)
		{
			while (result.MoveNext())
				yield return result.Current;

			try
			{
				GalleryLogger.SceneEnd("CommonSexPlayer", GetChars(pCommon, nCommon), GetInfos(sexType, state));

				switch ((CommonSexPlayerState)state)
				{
					case CommonSexPlayerState.Start:
						if (EventHandler.Npc.Id == nCommon.npcID)
							EventHandler?.AfterSex(null, pCommon, nCommon);
						else
							GalleryLogger.LogError($"CommonSexPlayer: NPC changed between start and end. ({EventHandler.Npc.Id} != {nCommon.npcID})");

						EventHandler = null;
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
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("CommonSexPlayer", error);
			}
			finally
			{
				if (state == (int)CommonSexPlayerState.Start)
				{
					GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(pCommon);
					GalleryScenesManager.Instance.RemoveSceneHandlerForCommon(nCommon);
				}
			}
		}
	}
}