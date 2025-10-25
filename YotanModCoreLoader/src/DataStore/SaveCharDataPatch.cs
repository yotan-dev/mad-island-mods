using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace YotanModCore.DataStore
{
	public class SaveCharDataPatch
	{
		private static readonly PropertyInfo CharaSaveModData = typeof(SaveManager.CharaSave).GetProperty("modData");

		/// <summary>
		/// Saves commonStates data into charaSave
		/// </summary>
		/// <param name="commonStates"></param>
		/// <param name="charaSave"></param>
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

		/// <summary>
		/// Loads commonStates data from charaSave
		/// </summary>
		/// <param name="commonStates"></param>
		/// <param name="charaSave"></param>
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
					PLogger.LogError($"Invalid custom data type: {SaveUtils.TryGetXmlNodeName(data)}. Ignoring it.");
					continue;
				}

				var store = commonStates.GetData(dataStore.GetStoreType());
				store.OnLoad(dataStore);
			}
		}

		/// <summary>
		/// Patches the Saving process to store custom data
		/// </summary>
		/// <param name="commonStates"></param>
		/// <param name="charaSave"></param>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.NPCCommonToCharaSave))]
		[HarmonyPostfix]
		private static void Post_SaveManager_NPCCommonToCharaSave(CommonStates nCommon, SaveManager.CharaSave chara)
		{
			SaveCommonData(nCommon, chara);
		}

		/// <summary>
		/// Patches the Loading process to load custom data
		/// </summary>
		/// <param name="commonStates"></param>
		/// <param name="charaSave"></param>
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.CharaSaveToNPCCommon))]
		[HarmonyPostfix]
		private static void Post_SaveManager_CharaSaveToNPCCommon(SaveManager.CharaSave npc, CommonStates nCommon)
		{
			LoadCommonData(nCommon, npc);
		}
	}
}
