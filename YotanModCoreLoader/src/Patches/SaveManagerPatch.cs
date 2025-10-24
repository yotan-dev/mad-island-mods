using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using YotanModCore.DataStore;

namespace YotanModCore.Patches
{
	public class SaveManagerPatch
	{
		private static readonly PropertyInfo GameModData = typeof(SaveManager.SaveEntry).GetProperty("modData");

		private static readonly PropertyInfo CharaSaveModData = typeof(SaveManager.CharaSave).GetProperty("modData");

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
					PLogger.LogError($"Invalid data type: {data.GetType()}");
					continue;
				}

				var store = Managers.mn.gameMN.GetDataStore<IGameDataStore>(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}

		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.NPCCommonToCharaSave))]
		[HarmonyPostfix]
		private static void Post_SaveManager_NPCCommonToCharaSave(
			CommonStates nCommon,
			SaveManager.CharaSave chara,
			SaveManager.NPCBagCommon npcBag
		)
		{
			var modData = new List<object>();
			CharaSaveModData.SetValue(chara, modData);

			var storeTypes = DataStoreManager.GetCommonSDataTypes();
			foreach (var storeType in storeTypes)
			{
				ICommonSDataStore store;
				if (!nCommon.TryGetData(storeType, out store))
					continue;

				modData.Add(store.OnSave());
			}
		}

		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.CharaSaveToNPCCommon))]
		[HarmonyPostfix]
		private static void Post_SaveManager_CharaSaveToNPCCommon(
			SaveManager.CharaSave npc,
			CommonStates nCommon,
			bool andMove,
			int id
		)
		{
			List<object> modData = CharaSaveModData.GetValue(npc) as List<object>;
			if (modData == null)
				modData = new List<object>();

			foreach (var data in modData)
			{
				var dataStore = data as ISaveData;
				if (dataStore == null)
				{
					PLogger.LogError($"Invalid data type: {data.GetType()}");
					continue;
				}

				var store = nCommon.GetData(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}
	}
}
