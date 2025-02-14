using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.CommonSexNPC;
using HarmonyLib;

namespace Gallery.Patches
{
	public class CommonSexNPCPatch
	{
		public struct CommonSexNpcInfo
		{
			public CommonStates NpcA;
			public CommonStates NpcB;
			public SexPlace SexPlace;
			public SexManager.SexCountState SexType;

			public CommonSexNpcInfo(CommonStates npcA, CommonStates npcB, SexPlace sexPlace, SexManager.SexCountState sexType)
			{
				this.NpcA = npcA;
				this.NpcB = npcB;
				this.SexPlace = sexPlace;
				this.SexType = sexType;
			}
		}

		public delegate void OnSceneInfo(CommonSexNpcInfo info);

		private static Dictionary<string, CommonStates> GetChars(CommonStates npcA, CommonStates npcB)
		{
			return new Dictionary<string, CommonStates>() {
				{"npcA", npcA},
				{"npcB", npcB},
			};
		}

		private static Dictionary<string, string> GetInfos(SexPlace sexPlace, SexManager.SexCountState sexType)
		{
			return new Dictionary<string, string>() {
				{ "sexPlace", $"{sexPlace?.name ?? "*null*"}, grade: {sexPlace?.grade.ToString() ?? "*null*"}, type: {sexPlace?.placeType.ToString() ?? "*null*"}" },
				{ "sexType", $"{sexType}" },
			};
		}

		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPrefix]
		private static void Pre_SexManager_CommonSexNPC(CommonStates npcA, CommonStates npcB, SexPlace sexPlace, SexManager.SexCountState sexType)
		{
			try
			{
				GalleryLogger.SceneStart("CommonSexNPC", GetChars(npcA, npcB), GetInfos(sexPlace, sexType));
				if (npcA == null || npcB == null)
				{
					PLogger.LogError("Skipping because npcA or npcB is null");
					return;
				}
				else if (npcA.employ == CommonStates.Employ.None && npcB.employ == CommonStates.Employ.None)
				{
					PLogger.LogInfo("Skipping because both are non-friend");
					return;
				}

				var tracker = new CommonSexNPCTracker(npcA, npcB, sexPlace, sexType);
				tracker.LoadPerformerId();
				GalleryScenesManager.Instance.AddTrackerForCommon(npcA, tracker);
				GalleryScenesManager.Instance.AddTrackerForCommon(npcB, tracker);
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("CommonSexNPC", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_CommonSexNPC(IEnumerator result, CommonStates npcA, CommonStates npcB, SexPlace sexPlace, SexManager.SexCountState sexType)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try
			{
				GalleryLogger.SceneEnd("CommonSexNPC", GetChars(npcA, npcB), GetInfos(sexPlace, sexType));
				if (npcA.employ == CommonStates.Employ.None && npcB.employ == CommonStates.Employ.None)
				{
					PLogger.LogInfo("Skipping because both are non-friend");
				}
				else
				{
					var handlerA = GalleryScenesManager.Instance.GetTrackerForCommon(npcA);
					handlerA?.End();

					var handlerB = GalleryScenesManager.Instance.GetTrackerForCommon(npcB);
					if (handlerB != handlerA)
						handlerB?.End();
				}
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("CommonSexNPC", error);
			}
			finally
			{
				GalleryScenesManager.Instance.RemoveTrackerForCommon(npcA);
				GalleryScenesManager.Instance.RemoveTrackerForCommon(npcB);
			}
		}
	}
}
