using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace YotanModCore
{
	/// <summary>
	/// Class to enable loading asset bundles from the CustomBundles folder.
	/// CustomBundles folder is meant as a general purpose folder for mods that don't need BepInEx,
	/// e.g. simple custom items, sex scripts, etc.
	/// </summary>
	[Experimental]
	public static class BundleLoader
	{
		internal static string BepInExRootPath;

		private static readonly Dictionary<string, AssetBundle> LoadedBundles = new();

		internal static void Load()
		{
			PLogger.LogInfo("Loading bundles");

			var bundlesRootDir = $"{BepInExRootPath}/CustomBundles";
			if (!Directory.Exists(bundlesRootDir))
				Directory.CreateDirectory(bundlesRootDir);

			var dllPaths = Directory.GetFiles(bundlesRootDir, "*.dll", SearchOption.AllDirectories);
			foreach (var dllPath in dllPaths)
			{
				PLogger.LogDebug($"Loading DLL {dllPath}");
				Assembly.LoadFile(dllPath);
			}

			var bundlePaths = Directory.GetFiles(bundlesRootDir, "*", SearchOption.AllDirectories);
			foreach (var bundlePath in bundlePaths)
			{
				if (bundlePath.EndsWith(".dll"))
					continue;

				PLogger.LogDebug($"Loading bundle {bundlePath}");

				var bundle = AssetBundle.LoadFromFile(bundlePath);
				LoadedBundles.Add(bundlePath, bundle);
			}
		}

		/// <summary>
		/// Loads all assets with the specified name and type from all loaded bundles.
		///
		/// Should be used after Start
		/// </summary>
		/// <typeparam name="T">The type of asset to load.</typeparam>
		/// <param name="assetName">The name of the asset to load.</param>
		/// <returns>A list of bundle assets.</returns>
		[Experimental]
		public static List<BundleAsset<T>> LoadAssetsWithName<T>(string assetName) where T : Object
		{
			var results = new List<BundleAsset<T>>();
			foreach (var bundle in LoadedBundles.Values)
			{
				var asset = bundle.LoadAsset<T>(assetName);
				if (asset != null)
					results.Add(new BundleAsset<T> { BundleName = bundle.name, Asset = asset });
			}

			return results;
		}

		/// <summary>
		/// Loads all assets of the specified type from all loaded bundles.
		///
		/// Should be used after Start
		/// </summary>
		/// <typeparam name="T">The type of asset to load.</typeparam>
		/// <returns>An array of bundle assets.</returns>
		[Experimental]
		public static BundleAsset<T>[] LoadAllAssetsOfType<T>() where T : Object
		{
			var results = new List<BundleAsset<T>>();
			foreach (var bundle in LoadedBundles.Values)
			{
				var assets = bundle.LoadAllAssets<T>();
				foreach (var asset in assets)
				{
					results.Add(new BundleAsset<T> { BundleName = bundle.name, Asset = asset });
				}
			}

			return results.ToArray();
		}
	}
}
