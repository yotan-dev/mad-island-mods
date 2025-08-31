using HarmonyLib;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace YotanModCore.Patches
{
	public class PropPanelsPatch
	{
		[HarmonyPatch(typeof(UIManager), "PropActionButton")]
		[HarmonyPrefix]
		private static bool Pre_UIManager_PropActionButton(int id, UIManager __instance)
		{
			if (__instance.propActProgress != PropPanelConst.Type.Custom)
				return true;

			PropPanelManager.Instance.OnButtonClicked(id);
			return false;
		}
	}
}