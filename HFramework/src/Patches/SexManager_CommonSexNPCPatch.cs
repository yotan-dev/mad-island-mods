using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore.Consts;
using HFramework.SexScripts;
using HFramework.SexScripts.Info;
using System.Collections.Generic;
using System;
using HFramework.Performer;

namespace HFramework.Patches
{
	public class SexManager_CommonSexNPCPatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_CommonSexNPC(
			CommonStates npcA,
			CommonStates npcB,
			SexPlace sexPlace,
			ref IEnumerator __result
		)
		{
			// @TODO: Probably a good idea to group Prefabs per type so we don't have to run through ALL scripts.

			List<Func<IEnumerator>> scripts = new List<Func<IEnumerator>>();

			var info = new CommonSexInfo
			{
				Place = sexPlace
			};

			BundleLoader.Loader.Prefabs
				.FindAll(p => p is CommonSexNPCScript && p.Info.CanStart([npcA, npcB]) && p.Info.CanExecute(info))
				.ForEach(p => scripts.Add(() => new TreeWrapper().Run(((CommonSexNPCScript) p).Create(npcA, npcB, sexPlace))));

			if (Config.Instance.EnableLegacyScenes.Value) {
				var legacyScene = new CommonSexNPC(npcA, npcB, sexPlace);
				if (ScenesManager.Instance.HasPerformer(legacyScene, PerformerScope.Sex, new CommonStates[] { npcA, npcB }))
				{
					scripts.Add(() => legacyScene.Run());
				}
			}

			if (scripts.Count > 0)
			{
				var targetScript = scripts[UnityEngine.Random.Range(0, scripts.Count)];
				__result = targetScript();
			}

			return false;
		}

		[HarmonyPatch(typeof(SexManager), "CommonSexNPC")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_CommonSexNPC(
			IEnumerator __result
		)
		{
			PLogger.LogDebug("Post_SexManager_CommonSexNPC Start");

			while (__result.MoveNext())
				yield return __result.Current;

			PLogger.LogDebug("Post_SexManager_CommonSexNPC End");
		}


	}
}
