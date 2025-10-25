namespace YotanModCore.DataStore
{
	/// <summary>
	/// Interface for a Common States (NPC/Player/etc) DataStore.
	/// Use to persist data related to common states.
	/// </summary>
	public interface ICommonSDataStore : IDataStore
	{
		/// <summary>
		/// Called when creating a new store for this CommonStates.
		/// This usually happens the first time GetData is called for this store in this CommonStates.
		/// </summary>
		/// <param name="commonStates"></param>
		public void Initialize(CommonStates commonStates);
	}
}
