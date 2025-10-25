using System.Collections.Generic;
using System.Reflection;
using System.Xml;
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

		private static string GetDataName(object data)
		{
			if (data is not XmlNode[] nodes)
				return "<Unknown Data>";

			if (nodes.Length == 0)
				return "<Unknown Empty>";

			return $"<{nodes[0].InnerText}>";
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
					PLogger.LogError($"Invalid custom data type: {GetDataName(data)}. Ignoring it.");
					continue;
				}

				var store = Managers.mn.gameMN.GetDataStore<IGameDataStore>(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}

		internal static void SaveCommonData(CommonStates commonStates, SaveManager.CharaSave charaSave)
		{
			var modData = new List<object>();
			CharaSaveModData.SetValue(charaSave, modData);

			var storeTypes = DataStoreManager.GetCommonSDataTypes();
			foreach (var storeType in storeTypes)
			{
				ICommonSDataStore store;
				if (!commonStates.TryGetData(storeType, out store))
					continue;

				modData.Add(store.OnSave());
			}
		}

		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.NPCCommonToCharaSave))]
		[HarmonyPostfix]
		private static void Post_SaveManager_NPCCommonToCharaSave(CommonStates nCommon, SaveManager.CharaSave chara)
		{
			SaveCommonData(nCommon, chara);
		}

		internal static void LoadCommonData(CommonStates commonStates, SaveManager.CharaSave charaSave)
		{
			List<object> modData = CharaSaveModData.GetValue(charaSave) as List<object>;
			if (modData == null)
				modData = new List<object>();

			foreach (var data in modData)
			{
				var dataStore = data as ISaveData;
				if (dataStore == null)
				{
					PLogger.LogError($"Invalid custom data type: {GetDataName(data)}. Ignoring it.");
					continue;
				}

				var store = commonStates.GetData(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}

		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.CharaSaveToNPCCommon))]
		[HarmonyPostfix]
		private static void Post_SaveManager_CharaSaveToNPCCommon(SaveManager.CharaSave npc, CommonStates nCommon)
		{
			LoadCommonData(nCommon, npc);
		}
	}
}
