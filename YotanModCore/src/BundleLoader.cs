using System.IO;
using UnityEngine;
using YotanModCore.Items;

namespace YotanModCore
{
	internal static class BundleLoader
	{
		internal static void Load()
		{
			PLogger.LogInfo("Loading bundles");

			if (!Directory.Exists("BepInEx/CustomBundles"))
				Directory.CreateDirectory("BepInEx/CustomBundles");

			string[] bundlePaths = Directory.GetFiles("BepInEx/CustomBundles", "*", SearchOption.AllDirectories);
			foreach (var bundlePath in bundlePaths)
			{
				PLogger.LogDebug($"Loading bundle {bundlePath}");

				var bundle = AssetBundle.LoadFromFile(bundlePath);
				var loader = bundle.LoadAsset<YMCDataLoader>("YMCDataLoader");
				if (loader == null)
				{
					PLogger.LogError($"Failed to load YMCDataLoader from bundle {bundlePath}. Make sure the asset exists with the right name.");
					continue;
				}

				var itemCount = 0;
				foreach (var item in loader.Items)
				{
					ItemDB.Instance.RegisterItem(item);
					itemCount++;
				}

				PLogger.LogDebug($"Loaded {itemCount} items from bundle {bundlePath}");
			}
		}
	}
}
