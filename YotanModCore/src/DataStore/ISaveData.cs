using System;

namespace YotanModCore.DataStore
{
	/// <summary>
	/// Interface for data that should be persisted into save files.
	///
	/// Mods that wants to use DataStores to persist custom data between playing sessions should implement
	/// their data containers by implementing this interface.
	/// </summary>
	public interface ISaveData
	{
		/// <summary>
		/// Returns the type of the data store that handles this data.
		/// The DataStore is the one responsible for handling load/save.
		/// </summary>
		/// <returns></returns>
		public Type GetStoreType();
	}
}
