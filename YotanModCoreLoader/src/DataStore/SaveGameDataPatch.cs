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

			var stores = Managers.mn.gameMN.GetDataStores();
			foreach (var store in stores)
				modData.Add(store.OnSave());
		}

		/// <summary>
		/// Patches the Loading process to load custom data
		/// </summary>
		/// <param name="__instance"></param>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadBuild), [])]
		[HarmonyPrefix]
		private static void Pre_SaveManager_LoadBuild(SaveManager __instance)
		{
			List<object> modData = GameModData.GetValue(__instance.saveDB.save[0]) as List<object>;
			if (modData == null)
				modData = new List<object>();

			foreach (var data in modData)
			{
				var dataStore = data as ISaveData;
				if (dataStore == null)
				{
					PLogger.LogError($"Invalid custom data type: {SaveUtils.TryGetXmlNodeName(data)}. Ignoring it.");
					continue;
				}

				var store = Managers.mn.gameMN.GetDataStore<IGameDataStore>(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}
	}
}
