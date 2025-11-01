namespace YotanModCore.DataStore
{
	/// <summary>
	/// Interface for a Global DataStore.
	/// Use to persist data related to the overall game, where only a single instance of it exists.
	///
	/// During the game lifecycle, one instance of this class will be created and kept.
	/// </summary>
	public interface IGameDataStore : IDataStore
	{
		/// <summary>
		/// Called when creating a new store for this GameManager.
		/// This usually happens the first time GetData is called for this store in this GameManager.
		/// A GameManager is recreated when the game restarts or a save file is loaded.
		/// </summary>
		/// <param name="gameMn"></param>
		public void Initialize(GameManager gameMn);
	}
}
