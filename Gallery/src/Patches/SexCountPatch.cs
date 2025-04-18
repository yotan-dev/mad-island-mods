using System;
using Gallery.GalleryScenes;
using HarmonyLib;
using YotanModCore;

namespace Gallery.Patches
{
	public class SexCountPatch
	{
		public struct SexCountChangeInfo
		{
			public CommonStates from;
			public CommonStates to;

			public SexCountChangeInfo(CommonStates from, CommonStates to)
			{
				this.from = from;
				this.to = to;
			}
		}

		private class DummyTracker : BaseTracker
		{
			public override void End() { }
		}

		private static DummyTracker DummyTrackerVal = new DummyTracker();

		public delegate void OnSexCount(object sender, SexCountChangeInfo value);

		public static event OnSexCount OnCreampie;

		public static event OnSexCount OnToilet;

		private static GalleryScenesManager GalleryManager { get { return GalleryScenesManager.Instance; } }

		[HarmonyPatch(typeof(SexManager), "SexCountChange")]
		[HarmonyPrefix]
		private static void Pre_SexManager_SexCountChange(CommonStates to, CommonStates from, SexManager.SexCountState sexState, ref bool __runOriginal)
		{
			if (Plugin.InGallery)
			{
				__runOriginal = false; // It messes up the execution in gallery. something is missing and it depends on
				return;
			}

			try
			{
				GalleryLogger.SexCountChanged(from, to, sexState, "");

				var trackerA = (from != null ? GalleryManager.GetTrackerForCommon(from) : null) ?? DummyTrackerVal;
				var trackerB = (to != null ? GalleryManager.GetTrackerForCommon(to) : null) ?? DummyTrackerVal;

				if (trackerA != trackerB)
				{
					var charaAName = CommonUtils.DetailedString(from);
					var charaBName = CommonUtils.DetailedString(to);
					GalleryLogger.LogError($"Pre_SexManager_SexCountChange: Found different trackers for {charaAName} and {charaBName} while changing sex count.");
				}

				CommonStates realFrom = from;
				CommonStates realTo = to;
				if (!CommonUtils.IsMale(from) && CommonUtils.IsMale(to))
				{
					realFrom = to;
					realTo = from;
				}

				switch (sexState)
				{
					case SexManager.SexCountState.Creampie:
						trackerA.DidCreampie = true;
						trackerB.DidCreampie = true;

						OnCreampie?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Delivery:
						trackerA.DidDelivery = true;
						trackerB.DidDelivery = true;
						break;

					case SexManager.SexCountState.Normal:
						trackerA.DidNormal = true;
						trackerB.DidNormal = true;
						break;

					case SexManager.SexCountState.Pregnant:
						trackerA.Pregnant = true;
						trackerB.Pregnant = true;
						break;

					case SexManager.SexCountState.Rapes:
						trackerA.Raped = true;
						trackerB.Raped = true;
						break;

					case SexManager.SexCountState.Toilet:
						trackerA.DidToilet = true;
						trackerB.DidToilet = true;
						OnToilet?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					default:
						GalleryLogger.LogDebug($"Pre_SexManager_SexCountChange: Unhandled sexState = {sexState}");
						break;
				}
			}
			catch (Exception error)
			{
				GalleryLogger.SceneErrorToPlayer("Pre_SexManager_SexCountChange", error);
			}
		}
	}
}
