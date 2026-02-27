using System;
using System.Collections;
using System.Collections.Generic;
using HFramework.Scenes;
using HarmonyLib;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using HFramework.SexScripts.Info;
using HFramework.SexScripts;

namespace HFramework.Patches
{
	public class SexManager_CommonSexPlayerPatch
	{
		[HarmonyPatch(typeof(SexManager), "CommonSexPlayer")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_CommonSexPlayer(
			int state,
			CommonStates pCommon,
			CommonStates nCommon,
			Vector3 pos,
			int sexType,
			ref IEnumerator __result
		)
		{
			// @TODO: Probably a good idea to group Prefabs per type so we don't have to run through ALL scripts.
			List<Func<IEnumerator>> scripts = [];
			var info = new PlayerSexInfo
			{
				Pos = pos,
				SexType = sexType,
			};

			// .CanStart ensures npcs are there, CanExecute checks for further conditions specific to the context.
			BundleLoader.Loader.Prefabs
				.FindAll(x => x is CommonSexPlayerScript && x.Info.CanStart(pCommon, nCommon) && x.Info.CanExecute(info))
				.ForEach(x => scripts.Add(() => new TreeWrapper().Run(((CommonSexPlayerScript)x).Create(pCommon, nCommon, pos, sexType))));

			if (Config.Instance.EnableLegacyScenes.Value)
			{
				var legacyScene = new CommonSexPlayer(pCommon, nCommon, pos, sexType);
				scripts.Add(() => legacyScene.Run());
			}

			if (scripts.Count > 0)
			{
				var targetScript = scripts[UnityEngine.Random.Range(0, scripts.Count)];
				__result = targetScript();
			}

			return false;
		}
	}
}
