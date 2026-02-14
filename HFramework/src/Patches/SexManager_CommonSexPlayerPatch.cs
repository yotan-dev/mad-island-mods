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
			PLogger.LogInfo("Common Sex Player");
			// if (nCommon.npcID == NpcID.FemaleNative && pCommon.npcID == NpcID.Man)
			// {
				var info = new PlayerSexInfo
				{
					Pos = pos,
					SexType = sexType,
				};

				// .CanStart ensures npcs are there, CanExecute checks for further conditions specific to the context.
				var tree = BundleLoader.Loader.Prefabs.Find(x => x is CommonSexPlayerScript && x.Info.CanStart(pCommon, nCommon) && x.Info.CanExecute(info)) as CommonSexPlayerScript;
				if (tree == null)
				{
					PLogger.LogError("Failed to load tree");
					return false;
				}

				PLogger.LogDebug($"Tree: {tree.Info.Npcs[0].NpcID} | {tree.Info.Npcs[1].NpcID}");

				var wrap = new TreeWrapper();
				__result = wrap.Run(tree.Create(pCommon, nCommon, pos, sexType));
				// var scr = new CommonSexNpcScript();
				// scr.Init(npcA, npcB, sexPlace);
				// var wrap = new SexScriptWrapper();
				// __result = wrap.Run(scr);
				return false;
			// }

			// var scene = new CommonSexPlayer(pCommon, nCommon, pos, sexType);
			// __result = scene.Run();
			// return false;
		}
	}
}
