using System.IO;
using System.Reflection;
using UnityEngine;
using YotanModCore.Items;

namespace HFramework
{
	internal static class BundleLoader
	{
		public static HFrameworkDataLoader Loader;

		internal static void Load()
		{
			PLogger.LogDebug($"Loading bundle HFramework");

			var bundle = AssetBundle.LoadFromFile("BepInEx/Plugins/HFramework/test.asset");
			Loader = bundle.LoadAsset<HFrameworkDataLoader>("HFrameworkDataLoader");
			if (Loader == null)
			{
				PLogger.LogError($"Failed to load HFrameworkDataLoader from bundle. Make sure the asset exists with the right name.");
				return;
			}

			PLogger.LogInfo($"Loaded HFrameworkDataLoader from bundle - {Loader.Prefabs.Count} prefabs");

			// var itemCount = 0;
			// foreach (var item in loader.Items)
			// {
			// 	ItemDB.Instance.RegisterItem(item);
			// 	itemCount++;
			// }

			// PLogger.LogDebug($"Loaded {itemCount} items from bundle {bundlePath}");

			// var recipesCount = 0;
			// foreach (var recipe in loader.CraftRecipes)
			// {
			// 	if (CraftDB.Instance.RegisterCraft(recipe))
			// 		recipesCount++;
			// }

			// PLogger.LogDebug($"Loaded {recipesCount} recipes from bundle {bundlePath}");
		}
	}
}
