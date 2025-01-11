using System.Collections;
using HFramework.Scenes;
using HarmonyLib;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Patches
{
	public class SexManager_SlavePatch
	{
		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Slave))]
		[HarmonyPrefix]
		private static bool Pre_SexManager_Slave(InventorySlot tmpSlave, ref IEnumerator __result)
		{
			var scene = new Slave(CommonUtils.GetActivePlayer(), tmpSlave);
			scene.Init(new DefaultSceneController());
			__result = scene.Run();
			return false;
		}

		[HarmonyPatch(typeof(SexManager), nameof(SexManager.Slave))]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_Slave(IEnumerator result)
		{
			while (result.MoveNext())
				yield return result.Current;

			// The original caller sets this and expects the scene to clean it up...
			// but this is not the responsability of the scene, so here we go.
			if (Managers.mn.uiMN.propActProgress == 5)
				Managers.mn.uiMN.propActProgress = PropPanelConst.Type.None;
		}
	}
}
