using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace IncreaseZoom
{
	
	[BepInPlugin("IncreaseZoom", "IncreaseZoom", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			Harmony.CreateAndPatchAll(typeof(Plugin));

			Logger.LogInfo($"Plugin Increase Zoom is loaded!");
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(FollowTarget), "Start")]
		private static void Post_FollowTarget_Start(FollowTarget __instance)
		{
			__instance.moveMin *= 2f;
			
			__instance.zoom.transform.Find("Main Camera").GetComponent<Camera>().farClipPlane *= 2;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(CullDistance), "Start")]
		private static void Pre_CullDistance_Start(CullDistance __instance)
		{
			for (int i = 0; i < __instance.cullDistance.Length; i++) {
				__instance.cullDistance[i] *= 2;
			}
		}
	}
}
