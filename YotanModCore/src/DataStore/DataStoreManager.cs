using System.Collections.Generic;
using System;

namespace YotanModCore.DataStore
{
	public static class DataStoreManager
	{
		/// <summary>
		/// Stores a map of StoreType => StoreFactory for GameManager
		/// </summary>
		private static readonly Dictionary<Type, Func<IGameDataStore>> gameDataStoreFactories = [];

		/// <summary>
		/// Stores a map of StoreType => StoreFactory for CommonStates
		/// </summary>
		private static readonly Dictionary<Type, Func<ICommonSDataStore>> commonSDataStoreFactories = [];

		/// <summary>
		/// Stores a list of types that should be saved, so we can patch XmlSerializer to support them.
		/// </summary>
		private static readonly List<Type> saveDataTypes = [];

		/// <summary>
		/// Creates a new GameDataStore for the given type and GameManager.
		/// </summary>
		/// <param name="storeType"></param>
		/// <param name="gameMn"></param>
		/// <returns></returns>
		internal static IGameDataStore CreateGameDataStore(Type storeType, GameManager gameMn)
		{
			if (!gameDataStoreFactories.ContainsKey(storeType))
				throw new Exception($"DataStoreManager: GameDataStore for type {storeType} not found");

			var store = gameDataStoreFactories[storeType]();
			store.Initialize(gameMn);

			return store;
		}

		/// <summary>
		/// Creates a new CommonSDataStore for the given type and CommonStates.
		/// </summary>
		/// <param name="storeType"></param>
		/// <param name="commonStates"></param>
		/// <returns></returns>
		internal static ICommonSDataStore CreateCommonSDataStore(Type storeType, CommonStates commonStates)
		{
			if (!commonSDataStoreFactories.ContainsKey(storeType))
				throw new Exception($"DataStoreManager: CommonSDataStore for type {storeType} not found");

			var store = commonSDataStoreFactories[storeType]();
			store.Initialize(commonStates);

			return store;
		}

		/// <summary>
		/// Registers a new GameManager DataStore (GameDataStore), which will use storeType as type and persist using saveType
		/// </summary>
		/// <param name="storeType"></param>
		/// <param name="dataStoreFactory"></param>
		/// <param name="saveType"></param>
		[Experimental]
		public static void RegisterDataStore(Type storeType, Func<IGameDataStore> dataStoreFactory, Type saveType)
		{
			if (storeType.GetInterface(typeof(IGameDataStore).FullName) is null)
			{
				PLogger.LogError($"DataStoreManager: Game DataStore for type {storeType} is not IGameDataStore", true);
				return;
			}

			if (saveType.GetInterface(typeof(ISaveData).FullName) is null)
			{
				PLogger.LogError($"DataStoreManager: Game DataStore for type {storeType} is not ISaveData", true);
				return;
			}

			if (gameDataStoreFactories.ContainsKey(storeType))
			{
				PLogger.LogError($"DataStoreManager: Game DataStore for type {storeType} already registered", true);
				return;
			}

			gameDataStoreFactories.Add(storeType, dataStoreFactory);
			saveDataTypes.Add(saveType);
		}

		/// <summary>
		/// Registers a new CommonStates DataStore (CommonSDataStore), which will use storeType as type and persist using dataType
		/// </summary>
		/// <param name="storeType"></param>
		/// <param name="dataStoreFactory"></param>
		/// <param name="dataType"></param>
		[Experimental]
		public static void RegisterDataStore(Type storeType, Func<ICommonSDataStore> dataStoreFactory, Type dataType)
		{
			if (storeType.GetInterface(typeof(ICommonSDataStore).FullName) is null)
			{
				PLogger.LogError($"DataStoreManager: CommonStates DataStore for type {storeType} is not ICommonSDataStore", true);
				return;
			}

			if (dataType.GetInterface(typeof(ISaveData).FullName) is null)
			{
				PLogger.LogError($"DataStoreManager: CommonStates DataStore for type {storeType} is not ISaveData", true);
				return;
			}

			if (commonSDataStoreFactories.ContainsKey(storeType))
			{
				PLogger.LogError($"DataStoreManager: CommonStates DataStore for type {storeType} already registered", true);
				return;
			}

			commonSDataStoreFactories.Add(storeType, dataStoreFactory);
			saveDataTypes.Add(dataType);
		}

		/// <summary>
		/// Returns a list of all types that should be saved, so we can patch XmlSerializer to support them.
		/// </summary>
		/// <returns></returns>
		[Experimental]
		public static Type[] GetSaveDataTypes()
		{
			return [.. saveDataTypes];
		}

		/// <summary>
		/// Returns a list of all types that are registered as GameManager DataStores (GameDataStore).
		/// </summary>
		/// <returns></returns>
		[Experimental]
		public static Type[] GetGameDataStoreTypes()
		{
			return [.. gameDataStoreFactories.Keys];
		}

		/// <summary>
		/// Returns a list of all types that are registered as CommonStates DataStores (CommonSDataStore).
		/// </summary>
		/// <returns></returns>
		[Experimental]
		public static Type[] GetCommonSDataStoreTypes()
		{
			return [.. commonSDataStoreFactories.Keys];
		}
	}
}
