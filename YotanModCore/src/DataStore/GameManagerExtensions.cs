using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace YotanModCore.DataStore
{
	public static class GameManagerExtensions
	{
		private static readonly Dictionary<Type, IGameDataStore> dataStores = new();

		internal static void AddDataStore(IGameDataStore dataStore)
		{
			dataStores.Add(dataStore.GetType(), dataStore);
		}

		internal static void ClearDataStores()
		{
			dataStores.Clear();
		}

		public static T GetDataStore<T>(this GameManager __instance) where T : class
		{
			if (!dataStores.ContainsKey(typeof(T)))
				throw new Exception($"DataStoreManager: DataStore for type {typeof(T)} not found");

			return dataStores[typeof(T)] as T;
		}

		public static T GetDataStore<T>(this GameManager __instance, Type dataType) where T : class
		{
			if (!dataStores.ContainsKey(dataType))
				throw new Exception($"DataStoreManager: DataStore for type {dataType} not found");

			return dataStores[dataType] as T;
		}

		public static ReadOnlyCollection<IGameDataStore> GetDataStores(this GameManager __instance)
		{
			return dataStores.Values.ToList().AsReadOnly();
		}
	}
}
