using System;
using System.Collections.Generic;
using System.Reflection;

namespace YotanModCore.DataStore
{
	public static class CommonStatesExtensions
	{
		private static readonly PropertyInfo CommonModData = typeof(CommonStates).GetProperty("modDataStores");

		public static T GetData<T>(this CommonStates __instance) where T : class
		{
			var dataStores = CommonModData.GetValue(__instance) as Dictionary<Type, object>;
			if (dataStores == null)
			{
				dataStores = new Dictionary<Type, object>();
				CommonModData.SetValue(__instance, dataStores);
			}

			if (!dataStores.ContainsKey(typeof(T)))
				dataStores.Add(typeof(T), DataStoreManager.CreateCommonSDataStore(typeof(T), __instance));

			return dataStores[typeof(T)] as T;
		}

		public static ICommonSDataStore GetData(this CommonStates __instance, Type storeType)
		{
			var dataStores = CommonModData.GetValue(__instance) as Dictionary<Type, object>;
			if (dataStores == null)
			{
				dataStores = new Dictionary<Type, object>();
				CommonModData.SetValue(__instance, dataStores);
			}

			if (!dataStores.ContainsKey(storeType))
				dataStores.Add(storeType, DataStoreManager.CreateCommonSDataStore(storeType, __instance));

			return dataStores[storeType] as ICommonSDataStore;
		}

		public static bool TryGetData<T>(this CommonStates __instance, Type storeType, out T store) where T : ICommonSDataStore
		{
			store = default;

			var dataStores = CommonModData.GetValue(__instance) as Dictionary<Type, object>;
			if (dataStores == null)
				return false;

			if (!dataStores.ContainsKey(storeType))
				return false;

			store = (T) dataStores[storeType];
			return true;
		}
	}
}
