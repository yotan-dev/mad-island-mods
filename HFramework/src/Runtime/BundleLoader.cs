using System;
using System.Linq;
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

		private static bool IsScriptValid(SexScripts.SexScript script, out string error) {
			error = null;
			try {
				if (script.Info == null)
					throw new Exception("Info is null");
				if (script.Info.Npcs == null || script.Info.Npcs.Length == 0)
					throw new Exception("No NPCs");

				foreach (var npc in script.Info.Npcs) {
					if (npc.Conditions == null)
						throw new Exception($"NPC {npc.NpcID} has NULL conditions");
				}
			} catch (Exception e) {
				error = e.Message;
			}

			return error == null;
		}

		private static void LoadSexScripts(SexScripts.SexScript[] scripts) {
			foreach (var script in scripts) {
				if (!string.IsNullOrEmpty(script.Info.MinimalGameVersion)) {
					if (GameInfo.GameVersion < GameInfo.ToVersion(script.Info.MinimalGameVersion)) {
						PLogger.LogWarning($"Skipping {script.name}: Minimal game version {script.Info.MinimalGameVersion} is higher than current game version {GameInfo.GameVersion}");
						continue;
					}
				}

				if (script.Info.RequiresDLC && !GameInfo.HasDLC) {
					PLogger.LogWarning($"Skipping {script.name}: Requires DLC but DLC is not available");
					continue;
				}

				if (!IsScriptValid(script, out string error)) {
					PLogger.LogError($"Skipping {script.name}: {error}");
					continue;
				}

				Loader.Prefabs.Add(script);
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
