using System.IO;
using System.Reflection;
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

			string[] dllPaths = Directory.GetFiles($"{BepInEx.Paths.BepInExRootPath}/CustomBundles", "*.dll", SearchOption.AllDirectories);
			foreach (var dllPath in dllPaths)
			{
				PLogger.LogDebug($"Loading DLL {dllPath}");
				Assembly.LoadFile(dllPath);
			}

			string[] bundlePaths = Directory.GetFiles($"{BepInEx.Paths.BepInExRootPath}/CustomBundles", "*", SearchOption.AllDirectories);
			foreach (var bundlePath in bundlePaths)
			{
				if (bundlePath.EndsWith(".dll"))
					continue;

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

				var recipesCount = 0;
				foreach (var recipe in loader.CraftRecipes)
				{
					if (CraftDB.Instance.RegisterCraft(recipe))
						recipesCount++;
				}

				PLogger.LogDebug($"Loaded {recipesCount} recipes from bundle {bundlePath}");
			}
		}
	}
}
