using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore.Consts;
using HFramework.SexScripts;
using HFramework.SexScripts.Info;

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
			if ((npcA.npcID == NpcID.MaleNative && npcB.npcID == NpcID.FemaleNative) ||
				(npcA.npcID == NpcID.FemaleNative && npcB.npcID == NpcID.MaleNative))
			{
				var info = new CommonSexInfo
				{
					Place = sexPlace
				};

				var tree = BundleLoader.Loader.Prefabs.Find(x => x.Info.CanExecute(info)) as CommonSexNPCScript;
				if (tree == null)
				{
					PLogger.LogError("Failed to load tree");
					return false;
				}

				var wrap = new TreeWrapper();
				__result = wrap.Run(tree.Create(npcA, npcB, sexPlace));
				// var scr = new CommonSexNpcScript();
				// scr.Init(npcA, npcB, sexPlace);
				// var wrap = new SexScriptWrapper();
				// __result = wrap.Run(scr);
				return false;
			}

			var scene = new CommonSexNPC(npcA, npcB, sexPlace);
			__result = scene.Run();
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
