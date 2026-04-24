using System;
using System.Linq;
using HFramework.SexScripts;
using UnityEngine;
using YotanModCore;

namespace HFramework
{
	[Experimental]
	internal static class BundleLoader
	{
		public static HFrameworkDataLoader Loader;

		private static void LoadBundle(string path) {
			PLogger.LogInfo($"Loading {path}");

			var bundle = AssetBundle.LoadFromFile(path);
			var assets = bundle.LoadAllAssets<SexScripts.SexScript>();

			LoadSexScripts(assets);
		}

		private static void LoadSexScripts(SexScripts.SexScript[] scripts) {
			foreach (var script in scripts) {
				if (SexScriptsManager.Instance.AddScript(script)) {
					// Temporary until we finish moving to SexScriptsMAnager
					Loader.Prefabs.Add(script);
				}
			}

			PLogger.LogInfo($"Loaded - {Loader.Prefabs.Count} prefabs");
		}

		internal static void Load() {
			Loader = HFrameworkDataLoader.CreateInstance<HFrameworkDataLoader>();

			LoadBundle("BepInEx/Plugins/HFramework/hframework");

			var assets = YotanModCore.BundleUtils.BundleLoader.LoadAllAssetsOfType<SexScripts.SexScript>();
			var scripts = assets.Select(x => x.Asset).ToArray();
			LoadSexScripts(scripts);
		}
	}
}
