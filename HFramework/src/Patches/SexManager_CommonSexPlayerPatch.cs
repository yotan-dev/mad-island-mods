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
			if (nCommon.npcID == NpcID.FemaleNative && pCommon.npcID == NpcID.Man)
			{
				var info = new PlayerSexInfo
				{
					Pos = pos,
					SexType = sexType,
				};

				var tree = BundleLoader.Loader.Prefabs.Find(x => x is CommonSexPlayerScript && x.Info.CanExecute(info)) as CommonSexPlayerScript;
				if (tree == null)
				{
					PLogger.LogError("Failed to load tree");
					return false;
				}

				var wrap = new TreeWrapper();
				__result = wrap.Run(tree.Create(pCommon, nCommon, pos, sexType));
				// var scr = new CommonSexNpcScript();
				// scr.Init(npcA, npcB, sexPlace);
				// var wrap = new SexScriptWrapper();
				// __result = wrap.Run(scr);
				return false;
			}

			var scene = new CommonSexPlayer(pCommon, nCommon, pos, sexType);
			__result = scene.Run();
			return false;
		}
	}
}
