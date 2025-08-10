using BepInEx;
using Gallery.Patches;
using Gallery.Patches.CommonSexPlayer;
using Gallery.SaveFile;
using HarmonyLib;
using HFramework.Hook;
using UnityEngine;
using YotanModCore;

namespace Gallery
{
	[BepInPlugin("Gallery", "Gallery", "1.0.4")]
	[BepInDependency("YotanModCore", "2.0.0")]
	[BepInDependency("HFramework", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static bool InGallery = false;

		private void Awake()
		{
			PLogger._Logger = Logger;

			Assets = AssetBundle.LoadFromFile($"BepInEx/plugins/Gallery/GalleryAssets.assets");
			
			Gallery.Config.Instance.Init(Config);
			GalleryLogger.Init();

			// If using Extended H-System replace mode, we don't have to patch the original code
			if (HFramework.Config.Instance.ReplaceOriginalScenes.Value) {
				HookManager.RegisterHooksEvent += GalleryHooks.Instance.InitHooks;
			} else {
				Harmony.CreateAndPatchAll(typeof(AssWallPatch));
				Harmony.CreateAndPatchAll(typeof(CommonSexNPCPatch));
				if (GameInfo.GameVersion <= GameInfo.ToVersion("0.0.12"))
					Harmony.CreateAndPatchAll(typeof(CommonSexPlayerPatchV0_012));
				else // if (GameInfo.GameVersion <= GameInfo.ToVersion("0.1.6"))
					Harmony.CreateAndPatchAll(typeof(CommonSexPlayerPatchV1_006));
				Harmony.CreateAndPatchAll(typeof(DarumaSexPatch));
				Harmony.CreateAndPatchAll(typeof(DeliveryPatch));
				Harmony.CreateAndPatchAll(typeof(ManRapesPatch));
				Harmony.CreateAndPatchAll(typeof(ManRapesSleepPatch));
				if (GameInfo.GameVersion >= GameInfo.ToVersion("0.1.0"))
					Harmony.CreateAndPatchAll(typeof(OnaniNpcPatch));
				Harmony.CreateAndPatchAll(typeof(SlavePatch));
				Harmony.CreateAndPatchAll(typeof(ToiletPatch));
				Harmony.CreateAndPatchAll(typeof(PlayerRapedPatch));
			}

			// Those are not handled by HFramework
			Harmony.CreateAndPatchAll(typeof(CommonRapesNPCPatch));
			Harmony.CreateAndPatchAll(typeof(ToiletNpcPatch));
			Harmony.CreateAndPatchAll(typeof(SexCountPatch));

			Harmony.CreateAndPatchAll(typeof(StoryPatches));
			Harmony.CreateAndPatchAll(typeof(TitleScreenPatch));
			Harmony.CreateAndPatchAll(typeof(UseLivePlacePatch));
			Harmony.CreateAndPatchAll(typeof(GalleryScenePatch));

			GalleryScenesManager.Instance.LoadGallery();
			GalleryState.Load();

			PLogger.LogInfo($"Plugin Gallery is loaded!");
		}
	}
}
