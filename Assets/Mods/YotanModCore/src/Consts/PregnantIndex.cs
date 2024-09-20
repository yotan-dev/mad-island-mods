namespace YotanModCore.Consts {
	/// <summary>
	/// Index for CommonStats.pregnant (except for PregnantIndex.None)
	/// </summary>
	public static class PregnantIndex {
		/// <summary>
		/// Used when Father is None/not pregnant
		/// </summary>
		public const int None = -1;

		/// <summary>
		/// NpcID of the father
		/// </summary>
		public const int Father = 0;

		/// <summary>
		/// Periods until the child is born
		/// </summary>
		public const int TimeToBirth = 1;
	}
}