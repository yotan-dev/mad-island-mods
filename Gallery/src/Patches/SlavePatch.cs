using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.GalleryScenes.Slave;
using HarmonyLib;
using YotanModCore;

namespace Gallery.Patches
{
	public class SlavePatch
	{
		private static SlaveTracker Tracker = null;

		private static Dictionary<string, CommonStates> GetCharas()
		{
			return new Dictionary<string, CommonStates>() { };
		}

		private static Dictionary<string, string> GetInfos(int state, InventorySlot tmpSlave)
		{
			string tmpSlaveStr = "null";
			if (tmpSlave != null)
			{
				tmpSlaveStr = $"{tmpSlave?.name ?? "null"}, type: {tmpSlave?.type.ToString() ?? "null"}, size: {tmpSlave?.size.ToString() ?? "null"}";
			}
			return new Dictionary<string, string>() {
				{ "state", $"{state}" },
				{ "tmpSlave", tmpSlaveStr },
			};
		}

		[HarmonyPatch(typeof(SexManager), "Slave")]
		[HarmonyPrefix]
		private static void Pre_SexManager_Slave(SexManager __instance, int state, InventorySlot tmpSlave = null)
		{
			if (Plugin.InGallery)
				return;

			try
			{
				GalleryLogger.SceneStart("Slave", GetCharas(), GetInfos(state, tmpSlave));

				if (state == 0)
				{
					Tracker = new SlaveTracker(CommonUtils.GetActivePlayer(), tmpSlave);
					Tracker.LoadPerformerId();

					GalleryScenesManager.Instance.AddTrackerForCommon(CommonUtils.GetActivePlayer(), Tracker);
				}
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("Slave", error);
			}
		}

		/// "From" rapes "to"
		[HarmonyPatch(typeof(SexManager), "Slave")]
		[HarmonyPostfix]
		private static IEnumerator Post_SexManager_Slave(IEnumerator result, int state, InventorySlot tmpSlave = null)
		{
			while (result.MoveNext())
				yield return result.Current;

			if (Plugin.InGallery)
				yield break;

			try
			{
				GalleryLogger.SceneEnd("Slave", GetCharas(), GetInfos(state, tmpSlave));

				if (state == 0 && tmpSlave != null)
				{
					ItemInfo component = tmpSlave.GetComponent<ItemInfo>();
					string itemKey = component.itemKey;

					Tracker?.End();
				}
				else if (state == 6 && Tracker != null)
				{
					Tracker.Busted = true;
				}
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("Slave", error);
			}
			finally
			{
				if (state == 0)
				{
					GalleryScenesManager.Instance.RemoveTrackerForCommon(CommonUtils.GetActivePlayer());
				}
			}
		}
	}
}
