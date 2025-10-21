using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace YotanModCore.DataStore
{
	public static class DataStoreManager
	{
		private static readonly Dictionary<Type, Func<IGameDataStore>> dataStoreFactories = new();

		internal static void Clear()
		{
			GameManagerExtensions.ClearDataStores();
		}

		internal static void LoadGameDataStores(GameManager __instance)
		{
			foreach (var dataStoreFactory in dataStoreFactories.Values)
			{
				var ds = dataStoreFactory();
				GameManagerExtensions.AddDataStore(ds);
			}
		}

		public static void RegisterDataStore(Type dataType, Func<IGameDataStore> dataStoreFactory)
		{
			if (dataStoreFactories.ContainsKey(dataType))
				throw new Exception($"DataStoreManager: DataStore for type {dataType} already registered");

			dataStoreFactories.Add(dataType, dataStoreFactory);
		}

		public static Type[] GetSaveDataTypes()
		{
			return dataStoreFactories.Keys.ToArray();
		}
	}
}
