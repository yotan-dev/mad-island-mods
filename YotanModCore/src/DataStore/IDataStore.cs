namespace YotanModCore.DataStore
{
	/// <summary>
	/// Interface for a Global DataStore.
	/// Use to persist data related to the overall game, where only a single instance of it exists.
	///
	/// During the game lifecycle, one instance of this class will be created and kept.
	/// </summary>
	public interface IDataStore
	{
		/// <summary>
		/// Called when the data is loaded from the save file.
		/// </summary>
		/// <param name="data">The data loaded from the save file.</param>
		public abstract void OnLoad(ISaveData data);

		/// <summary>
		/// Called when the data is saved to the save file.
		/// </summary>
		/// <returns>The data to be saved to the save file.</returns>
		public abstract ISaveData OnSave();
	}
}
