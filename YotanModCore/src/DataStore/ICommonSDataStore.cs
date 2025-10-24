namespace YotanModCore.DataStore
{
	/// <summary>
	/// Interface for a Common States (NPC/Player/etc) DataStore.
	/// Use to persist data related to common states.
	/// </summary>
	public interface ICommonSDataStore
	{
		public void Initialize(CommonStates commonStates);

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
