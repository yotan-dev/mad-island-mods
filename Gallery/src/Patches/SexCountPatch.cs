using System;
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

		public delegate void OnSexCount(object sender, SexCountChangeInfo value);

		public static event OnSexCount OnCreampie;
		
		public static event OnSexCount OnDelivery;

		public static event OnSexCount OnNormal;
		
		public static event OnSexCount OnPregnant;
		
		public static event OnSexCount OnRape;
		
		public static event OnSexCount OnToilet;

		private static GalleryScenesManager GalleryManager { get { return GalleryScenesManager.Instance; } }

		[HarmonyPatch(typeof(SexManager), "SexCountChange")]
		[HarmonyPrefix]
		private static void Pre_SexManager_SexCountChange(CommonStates to, CommonStates from, SexManager.SexCountState sexState, ref bool __runOriginal)
		{
			if (Plugin.InGallery) {
				__runOriginal = false; // It messes up the execution in gallery. something is missing and it depends on
				return;
			}

			try {
				GalleryLogger.SexCountChanged(from, to, sexState, "");

				var handlerA = GalleryManager.GetSceneHandlerForCommon(from);
				var handlerB = GalleryManager.GetSceneHandlerForCommon(to);
				if (handlerA != handlerB) {
					var charaAName = CommonUtils.DetailedString(from);
					var charaBName = CommonUtils.DetailedString(to);
					GalleryLogger.LogError($"Pre_SexManager_SexCountChange: Found different scenes for {charaAName} and {charaBName} while changing sex count.");
				}

				CommonStates realFrom = from;
				CommonStates realTo = to;
				if (!CommonUtils.IsMale(from) && CommonUtils.IsMale(to)) {
					realFrom = to;
					realTo = from;
				}

				switch (sexState)
				{
					case SexManager.SexCountState.Creampie:
						handlerA?.OnCreampie(realFrom, realTo);
						if (handlerA != handlerB)
							handlerB?.OnCreampie(realFrom, realTo);

						OnCreampie?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Delivery:
						// handlerA?.OnDelivery();
						// handlerB?.OnDelivery();
						OnDelivery?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Normal:
						handlerA?.OnNormalSex(realFrom, realTo);
						if (handlerA != handlerB)
							handlerB?.OnNormalSex(realFrom, realTo);
						OnNormal?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Pregnant:
						// handlerA?.OnPregnantCount();
						// handlerB?.OnPregnantCount();
						OnPregnant?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Rapes:
						// handlerA?.OnRape(realFrom, realTo);
						// if (handlerA != handlerB)
						// 	handlerB?.OnRape(realFrom, realTo);
						OnRape?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Toilet:
						// handlerA?.OnToiletCount();
						// handlerB?.OnToiletCount();
						OnToilet?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					default:
						GalleryLogger.LogDebug($"Pre_SexManager_SexCountChange: Unhandled sexState = {sexState}");
						break;
				}
			} catch (Exception error) {
				GalleryLogger.SceneErrorToPlayer("Pre_SexManager_SexCountChange", error);
			}
		}
	}
}