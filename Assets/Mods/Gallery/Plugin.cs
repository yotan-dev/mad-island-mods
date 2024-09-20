using BepInEx;
using Gallery.Patches;
using Gallery.Patches.CommonSexPlayer;
using Gallery.SaveFile;
using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace Gallery
{
	[BepInPlugin("Gallery", "Gallery", "0.1.0")]
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
			GalleryScenesManager.Init();
			
			Harmony.CreateAndPatchAll(typeof(AssWallPatch));
			Harmony.CreateAndPatchAll(typeof(CommonRapesNPCPatch));
			Harmony.CreateAndPatchAll(typeof(CommonSexNPCPatch));
			if (GameInfo.GameVersion <= GameInfo.ToVersion("0.0.12"))
				Harmony.CreateAndPatchAll(typeof(CommonSexPlayerPatchV0_012));
			else // if (GameInfo.GameVersion <= GameInfo.ToVersion("0.1.6"))
				Harmony.CreateAndPatchAll(typeof(CommonSexPlayerPatchV1_006));
			Harmony.CreateAndPatchAll(typeof(DarumaSexPatch));
			Harmony.CreateAndPatchAll(typeof(DeliveryPatch));
			Harmony.CreateAndPatchAll(typeof(ManRapesPatch));
			Harmony.CreateAndPatchAll(typeof(ManRapesSleepPatch));
			Harmony.CreateAndPatchAll(typeof(PlayerRapedPatch));
			Harmony.CreateAndPatchAll(typeof(PlaySexPatch));
			Harmony.CreateAndPatchAll(typeof(PropPanelPatch));
			Harmony.CreateAndPatchAll(typeof(SexCountPatch));
			Harmony.CreateAndPatchAll(typeof(SlavePatch));
			Harmony.CreateAndPatchAll(typeof(StoryPatches));
			Harmony.CreateAndPatchAll(typeof(TitleScreenPatch));
			Harmony.CreateAndPatchAll(typeof(ToiletNpcPatch));
			Harmony.CreateAndPatchAll(typeof(ToiletPatch));
			Harmony.CreateAndPatchAll(typeof(UseLivePlacePatch));

			if (GameInfo.GameVersion >= GameInfo.ToVersion("0.1.0"))
				Harmony.CreateAndPatchAll(typeof(OnaniNpcPatch));
		
			GalleryState.Load();

			PLogger.LogInfo($"Plugin Gallery is loaded!");
		}
	}
}
