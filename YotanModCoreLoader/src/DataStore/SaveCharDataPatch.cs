using System;
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
			try
			{
				var modData = new List<object>();
				CharaSaveModData.SetValue(charaSave, modData);

				foreach (var storeType in DataStoreManager.GetCommonSDataStoreTypes())
				{
					if (!commonStates.TryGetData(storeType, out ICommonSDataStore store))
						continue;

					try
					{
						modData.Add(store.OnSave());
					}
					catch (Exception e)
					{
						PLogger.LogError($"SaveCharDataPatch: Failed to save CommonStates MOD data for NPC ID {commonStates.npcID} / Data kind: {storeType}");
						PLogger.LogError("Skipping data realted to this kind to prevent save corruption.");
						PLogger.LogError(e);
					}
				}
			}
			catch (Exception e)
			{
				PLogger.LogError($"SaveCharDataPatch: Failed to save CommonStates MOD data for NPC ID {commonStates.npcID}");
				PLogger.LogError("Skipping all of its MOD PROVIDED data to prevent save corruption.");
				PLogger.LogError(e);
			}
		}

		/// <summary>
		/// Loads commonStates data from charaSave
		/// </summary>
		/// <param name="commonStates"></param>
		/// <param name="charaSave"></param>
		internal static void LoadCommonData(CommonStates commonStates, SaveManager.CharaSave charaSave)
		{
			if (CharaSaveModData.GetValue(charaSave) is not List<object> modData)
				modData = [];

			foreach (var data in modData)
			{
				if (data is not ISaveData dataStore)
				{
					PLogger.LogError($"Invalid custom data type: {SaveUtils.TryGetXmlNodeName(data)}. Ignoring it. (Probably missing/removed/broken mod)");
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
