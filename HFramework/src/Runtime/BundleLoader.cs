using System;
using System.IO;
using System.Linq;
using HFramework.SexScripts;
using UnityEngine;

namespace HFramework
{
	[Experimental]
	internal static class BundleLoader
	{
		private static void LoadBundle(string path) {
			try {
				var bundle = AssetBundle.LoadFromFile(path);
				var assets = bundle.LoadAllAssets<SexScript>();

				LoadSexScripts(assets);
			} catch (Exception e) {
				PLogger.LogError($"Failed to default sex scripts bundle at \"{path}\". Is your installation broken?");
				PLogger.LogError($"Error: {e.Message}");
				PLogger.LogError(e);
			}
		}

		private static void LoadSexScripts(SexScript[] scripts) {
			foreach (var script in scripts) {
				SexScriptsManager.Instance.AddScript(script);
			}
		}

		internal static void Load() {
			LoadBundle(Path.Combine(Initializer.HFrameworkFolderPath, "hframework"));

			var assets = YotanModCore.BundleUtils.BundleLoader.LoadAllAssetsOfType<SexScript>();
			var scripts = assets.Select(x => x.Asset).ToArray();
			LoadSexScripts(scripts);
		}
	}
}
