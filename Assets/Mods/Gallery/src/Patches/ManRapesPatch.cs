using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;

namespace Gallery.Patches
{
	public class ManRapesPatch
	{
		public struct ManRapesInfo
		{
			public CommonStates man;
			public CommonStates girl;

			public ManRapesInfo(CommonStates man, CommonStates girl) {
				this.man = man;
				this.girl = girl;
			}
		}

		public delegate void OnSceneInfo(ManRapesInfo e);

		public static event OnSceneInfo OnStart;
		
		public static event OnSceneInfo OnEnd;

		private static Dictionary<string, CommonStates> Getcharas(CommonStates girl, CommonStates man) {
			return new Dictionary<string, CommonStates>() {
				{ "man (rapist)", man },
				{ "girl (victim)", girl },
			};
		}

		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPrefix]
		private static void Pre_SexManager_ManRapes(CommonStates girl, CommonStates man)
		{
			if (Plugin.InGallery)
				return;

			try {
				GalleryLogger.SceneStart("ManRapes", Getcharas(girl, man), new Dictionary<string, string>());
				OnStart?.Invoke(new ManRapesInfo(man, girl));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapes", error);
			}
		}

		[HarmonyPatch(typeof(SexManager), "ManRapes")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_ManRapes(IEnumerator result, CommonStates girl, CommonStates man)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try {
				GalleryLogger.SceneEnd("ManRapes", Getcharas(girl, man), new Dictionary<string, string>());
				OnEnd?.Invoke(new ManRapesInfo(man, girl));
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("ManRapes", error);
			}
		}
	}
}
