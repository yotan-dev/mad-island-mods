using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace YotanModCore.DataStore
{
	/// <summary>
	/// Patches SaveManager to save and load game "world" data
	///
	/// World data is saved into "SaveEntry::modData", which is created by YotanModCore Patcher
	/// </summary>
	public class SaveGameDataPatch
	{
		private static readonly PropertyInfo GameModData = typeof(SaveManager.SaveEntry).GetProperty("modData");

		/// <summary>
		/// Patches the Saving process to store custom data
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.TempToSave))]
		[HarmonyPrefix]
		private static void Pre_SaveManager_TempToSave(SaveManager __instance)
		{
			try
			{
				var modData = new List<object>();
				GameModData.SetValue(__instance.saveEntry, modData);

				foreach (var storeType in DataStoreManager.GetGameDataStoreTypes())
				{
					if (!Managers.mn.gameMN.TryGetData(storeType, out IGameDataStore store))
						continue;

					try
					{
						modData.Add(store.OnSave());
					}
					catch (Exception e)
					{
						PLogger.LogError($"SaveGameDataPatch: Failed to save Game MOD data / Data kind: {storeType}");
						PLogger.LogError("Skipping data related to this kind to prevent save corruption.");
						PLogger.LogError(e);
					}
				}
			}
			catch (Exception e)
			{
				PLogger.LogError($"SaveGameDataPatch: Failed to save Game MOD data");
				PLogger.LogError("Skipping all of its MOD PROVIDED data to prevent save corruption.");
				PLogger.LogError(e);
			}
		}

		/// <summary>
		/// Patches the Loading process to load custom data
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadBuild), [])]
		[HarmonyPrefix]
		private static void Pre_SaveManager_LoadBuild(SaveManager __instance)
		{
			if (GameModData.GetValue(__instance.saveDB.save[0]) is not List<object> modData)
				modData = [];

			foreach (var data in modData)
			{
				if (data is not ISaveData dataStore)
				{
					PLogger.LogError($"Invalid custom data type: {SaveUtils.TryGetXmlNodeName(data)}. Ignoring it. (Probably missing/removed/broken mod)");
					continue;
				}

				var store = Managers.mn.gameMN.GetData(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}
	}
}
