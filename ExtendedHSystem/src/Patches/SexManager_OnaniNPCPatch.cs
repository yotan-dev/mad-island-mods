using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace ExtendedHSystem.Patches
{
	public class SexManager_OnaniNPCPatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.OnaniNPC))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(CommonStates common, SexPlace sexPlace, float upMoral, ref IEnumerator __result)
		{
			var scene = new OnaniNPC(common, sexPlace, upMoral);
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();
			return false;
		}
	}
}
