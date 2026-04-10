using System.IO;
using HFramework.SexScripts;
using UnityEngine;

namespace HFramework
{
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

			foreach (var item in assets)
			{
				Loader.Prefabs.Add(item);
			}

			PLogger.LogInfo($"Loaded - {Loader.Prefabs.Count} prefabs");
		}

		internal static void Load() {
			Loader = HFrameworkDataLoader.CreateInstance<HFrameworkDataLoader>();

			LoadBundle("BepInEx/Plugins/HFramework/hf_sex_scripts");

			if (!Directory.Exists("BepInEx/Plugins/HFramework/CustomBundles"))
				Directory.CreateDirectory("BepInEx/Plugins/HFramework/CustomBundles");

			string[] bundlePaths = Directory.GetFiles($"BepInEx/Plugins/HFramework/CustomBundles", "*", SearchOption.AllDirectories);
			foreach (var bundlePath in bundlePaths) {
				if (bundlePath.EndsWith(".dll"))
					continue;

				LoadBundle(bundlePath);
			}
		}
	}
}
