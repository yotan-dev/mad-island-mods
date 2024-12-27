using System.Collections;
using ExtendedHSystem.Scenes;
using HarmonyLib;

namespace ExtendedHSystem.Patches
{
	public class SexManager_DeliveryPatch
	{
		[HarmonyPatch(typeof(SexManager), "Delivery")]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Delivery(CommonStates common, WorkPlace tmpWorkPlace, SexPlace tmpSexPlace, ref IEnumerator __result)
		{
			var scene = new Delivery(common, tmpWorkPlace, tmpSexPlace);
			scene.Init(new DefaultSceneController());
			scene.AddEventHandler(new DefaultSceneEventHandler());
			__result = scene.Run();
			return false;
		}
	}
}
