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

				var sceneA = GalleryManager.GetSceneWithChara(from);
				var sceneB = GalleryManager.GetSceneWithChara(to);
				if (sceneA != sceneB) {
					var charaAName = CommonUtils.DetailedString(from);
					var charaBName = CommonUtils.DetailedString(to);
					GalleryLogger.LogError($"Pre_SexManager_SexCountChange: Found different scenes for {charaAName} and {charaBName} while changing sex count.");
				}

				switch (sexState)
				{
					case SexManager.SexCountState.Creampie:
						sceneA?.OnCreampieCount();
						sceneB?.OnCreampieCount();
						OnCreampie?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Delivery:
						sceneA?.OnDeliveryCount();
						sceneB?.OnDeliveryCount();
						OnDelivery?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Normal:
						sceneA?.OnNormalCount();
						sceneB?.OnNormalCount();
						OnNormal?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Pregnant:
						sceneA?.OnPregnantCount();
						sceneB?.OnPregnantCount();
						OnPregnant?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Rapes:
						sceneA?.OnRapeCount();
						sceneB?.OnRapeCount();
						OnRape?.Invoke(null, new SexCountChangeInfo(from, to));
						break;

					case SexManager.SexCountState.Toilet:
						sceneA?.OnToiletCount();
						sceneB?.OnToiletCount();
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