using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.Onani;
using HarmonyLib;
using YotanModCore.Consts;

namespace Gallery.Patches
{
	public class OnaniNpcPatch
	{
		private static Dictionary<string, CommonStates> GetChars(CommonStates npc)
		{
			return new Dictionary<string, CommonStates>() {
				{"npc", npc},
			};
		}

		private static Dictionary<string, string> GetInfos(SexPlace sexPlace)
		{
			return new Dictionary<string, string>() {
				{ "sexPlace", $"{sexPlace?.name ?? "*null*"}, grade: {sexPlace?.grade.ToString() ?? "*null*"}, type: {sexPlace?.placeType.ToString() ?? "*null*"}" },
			};
		}

		[HarmonyPatch(typeof(SexManager), "OnaniNPC")]
		[HarmonyPrefix]
		private static void Pre_SexManager_OnaniNPC(CommonStates common, SexPlace sexPlace)
		{
			if (Plugin.InGallery)
				return;

			try
			{
				GalleryLogger.SceneStart("OnaniNPC", GetChars(common), GetInfos(sexPlace));
				if (common == null)
				{
					PLogger.LogError("Skipping because npc is null");
					return;
				}
				else if (common.employ == CommonStates.Employ.None)
				{
					PLogger.LogInfo("Skipping because NPC is non-friend");
					return;
				}

				if (common.employ != CommonStates.Employ.None)
				{
					var tracker = new OnaniTracker(common);
					tracker.LoadPerformerId();
					GalleryScenesManager.Instance.AddTrackerForCommon(common, tracker);
				}
				else
				{
					PLogger.LogInfo("Skipping because NPC is non-friend");
				}
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("OnaniNPC", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "OnaniNPC")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_OnaniNPC(IEnumerator result, CommonStates common, SexPlace sexPlace)
		{
			var masturbationCount = common.sexInfo[SexInfoIndex.Masturbation];
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try
			{
				GalleryLogger.SceneEnd("OnaniNPC", GetChars(common), GetInfos(sexPlace));
				if (common.employ != CommonStates.Employ.None)
				{
					var tracker = GalleryScenesManager.Instance.GetTrackerForCommon(common);
					if (tracker != null && common.sexInfo[SexInfoIndex.Masturbation] > masturbationCount)
						tracker.DidMasturbation = true;
					tracker?.End();
				}
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("OnaniNPC", error);
			}
			finally
			{
				GalleryScenesManager.Instance.RemoveTrackerForCommon(common);
			}
		}
	}
}
