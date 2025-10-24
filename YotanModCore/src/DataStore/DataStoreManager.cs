using System.Collections.Generic;
using System;
using System.Linq;

namespace YotanModCore.DataStore
{
	public static class DataStoreManager
	{
		private static readonly Dictionary<Type, Func<IGameDataStore>> gameDataStoreFactories = new();

		private static readonly Dictionary<Type, Func<ICommonSDataStore>> commonSDataStoreFactories = new();

		private static readonly List<Type> saveDataTypes = new();

		internal static void Clear()
		{
			GameManagerExtensions.ClearDataStores();
		}

		internal static void LoadGameDataStores(GameManager __instance)
		{
			foreach (var dataStoreFactory in gameDataStoreFactories.Values)
			{
				var ds = dataStoreFactory();
				GameManagerExtensions.AddDataStore(ds);
			}
		}

		internal static ICommonSDataStore CreateCommonSDataStore(Type storeType, CommonStates commonStates)
		{
			if (!commonSDataStoreFactories.ContainsKey(storeType))
				throw new Exception($"DataStoreManager: CommonSDataStore for type {storeType} not found");

			var store = commonSDataStoreFactories[storeType]();
			store.Initialize(commonStates);

			return store;
		}

		public static void RegisterDataStore(Type dataType, Func<IGameDataStore> dataStoreFactory)
		{
			if (gameDataStoreFactories.ContainsKey(dataType))
				throw new Exception($"DataStoreManager: DataStore for type {dataType} already registered");

			gameDataStoreFactories.Add(dataType, dataStoreFactory);
			saveDataTypes.Add(dataType);
		}

		public static void RegisterDataStore(Type storeType, Func<ICommonSDataStore> dataStoreFactory, Type dataType)
		{
			if (commonSDataStoreFactories.ContainsKey(storeType))
				throw new Exception($"DataStoreManager: DataStore for type {storeType} already registered");

			commonSDataStoreFactories.Add(storeType, dataStoreFactory);
			saveDataTypes.Add(dataType);
		}

		public static Type[] GetSaveDataTypes()
		{
			return saveDataTypes.ToArray();
		}

		public static Type[] GetCommonSDataTypes()
		{
			return commonSDataStoreFactories.Keys.ToArray();
		}
	}
}
