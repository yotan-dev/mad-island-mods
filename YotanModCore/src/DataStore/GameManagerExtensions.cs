using System;
using System.Collections.Generic;
using System.Reflection;

namespace YotanModCore.DataStore
{
	public static class GameManagerExtensions
	{
		private static readonly PropertyInfo GameModData = typeof(GameManager).GetProperty("modDataStores");

		[Experimental]
		public static T GetData<T>(this GameManager __instance) where T : class
		{
			if (GameModData.GetValue(__instance) is not Dictionary<Type, object> dataStores)
			{
				dataStores = new Dictionary<Type, object>();
				GameModData.SetValue(__instance, dataStores);
			}

			if (!dataStores.ContainsKey(typeof(T)))
				dataStores.Add(typeof(T), DataStoreManager.CreateGameDataStore(typeof(T), __instance));

			return dataStores[typeof(T)] as T;
		}

		[Experimental]
		public static IGameDataStore GetData(this GameManager __instance, Type storeType)
		{
			PLogger.LogInfo($"GameManager: {__instance == null}");
			PLogger.LogInfo($"GetData: {storeType}");
			PLogger.LogInfo($"GameModData: {GameModData.GetValue(__instance)}");
			if (GameModData.GetValue(__instance) is not Dictionary<Type, object> dataStores)
			{
				PLogger.LogInfo($"GameModData is null");
				dataStores = new Dictionary<Type, object>();
				GameModData.SetValue(__instance, dataStores);
			}

			PLogger.LogInfo($"dataStores: {dataStores}");
			if (!dataStores.ContainsKey(storeType))
				dataStores.Add(storeType, DataStoreManager.CreateGameDataStore(storeType, __instance));

			return dataStores[storeType] as IGameDataStore;
		}

		[Experimental]
		public static bool TryGetData<T>(this GameManager __instance, Type storeType, out T store) where T : IGameDataStore
		{
			store = default;

			if (GameModData.GetValue(__instance) is not Dictionary<Type, object> dataStores)
				return false;

			if (!dataStores.ContainsKey(storeType))
				return false;

			store = (T)dataStores[storeType];
			return true;
		}
	}
}
