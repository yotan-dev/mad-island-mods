using System.Collections;
using HarmonyLib;
using YotanModCore;
using Gallery.UI.SceneController;
using YotanModCore.Consts;

namespace Gallery.Patches
{
	public class PropPanelPatch
	{
		public delegate void OnPropActionEvent(int propActProgress, int id, int state);

		public static event OnPropActionEvent OnPropAction;

		[HarmonyPatch(typeof(UIManager), "PropPanelOpen")]
		[HarmonyPostfix]
		private static void Post_UIManager_PropPanelOpen(int actProgress, UIManager __instance)
		{
			if (actProgress == PropPanelConst.Type.SleepRapeGallery) {
				__instance.PropPanelStateChange(0, PropPanelConst.Text.ForcefullyRape, ManRapeSleepConst.Actions.Rape, true);
				__instance.PropPanelStateChange(1, PropPanelConst.Text.UseSleepPowder, ManRapeSleepConst.Actions.SleepPowder, false);
				__instance.PropPanelStateChange(2, PropPanelConst.Text.GentlyRape, ManRapeSleepConst.Actions.DiscretlyRape, true);
			}
		}

		[HarmonyPatch(typeof(UIManager), "PropAction")]
		[HarmonyPostfix]
		private static IEnumerator Post_UIManager_PropAction(IEnumerator result, int id, UIManager __instance)
		{
			while (result.MoveNext())
				yield return result.Current;

			int actionState = Managers.mn.uiMN.propActionState[id];
			if (__instance.propActProgress == PropPanelConst.Type.SleepRapeGallery) {
				if (actionState == ManRapeSleepConst.Actions.SleepPowder)
					yield return ManRapesSleepScene.Instance.ManRapesSleep(ManRapeSleepConst.StartDiscretlyRape, null, null, 0);
				else if (actionState == ManRapeSleepConst.Actions.DiscretlyRape)
					yield return ManRapesSleepScene.Instance.ManRapesSleep(ManRapeSleepConst.StartDiscretlyRape, null, null, 1);
				else
					yield return ManRapesSleepScene.Instance.ManRapesSleep(actionState, null, null, 0);
			}

			OnPropAction?.Invoke(__instance.propActProgress, id, actionState);
		}
	}
}