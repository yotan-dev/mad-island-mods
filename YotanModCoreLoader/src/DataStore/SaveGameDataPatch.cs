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
			var modData = new List<object>();
			GameModData.SetValue(__instance.saveEntry, modData);

			foreach (var storeType in DataStoreManager.GetGameDataStoreTypes())
			{
				if (!Managers.mn.gameMN.TryGetData(storeType, out IGameDataStore store))
					continue;

				modData.Add(store.OnSave());
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
					PLogger.LogError($"Invalid custom data type: {SaveUtils.TryGetXmlNodeName(data)}. Ignoring it.");
					continue;
				}

				var store = Managers.mn.gameMN.GetData(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}
	}
}
