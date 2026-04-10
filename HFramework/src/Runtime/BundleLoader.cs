using System.Linq;
using HFramework.SexScripts;
using UnityEngine;

namespace HFramework
{
	[Experimental]
	internal static class BundleLoader
	{
		public static HFrameworkDataLoader Loader;

		private static void LoadBundle(string path) {
			PLogger.LogInfo($"Loading {path}");

			var bundle = AssetBundle.LoadFromFile(path);
			var assets = bundle.LoadAllAssets<SexScript>();
			if (Loader == null) {
				PLogger.LogError($"Failed to load HFrameworkDataLoader from bundle. Make sure the asset exists with the right name.");
				return;
			}

			LoadSexScripts(assets);
		}

		private static void LoadSexScripts(SexScript[] scripts) {
			foreach (var item in scripts) {
				Loader.Prefabs.Add(item);
			}

			PLogger.LogInfo($"Loaded - {Loader.Prefabs.Count} prefabs");
		}

		internal static void Load() {
			Loader = HFrameworkDataLoader.CreateInstance<HFrameworkDataLoader>();

			// Temp disable until we start making the official scripts
			// LoadBundle("BepInEx/Plugins/HFramework/hf_sex_scripts");

			var assets = YotanModCore.BundleLoader.LoadAllAssetsOfType<SexScript>();
			var scripts = assets.Select(x => x.Asset).ToArray();
			LoadSexScripts(scripts);
		}
	}
}
