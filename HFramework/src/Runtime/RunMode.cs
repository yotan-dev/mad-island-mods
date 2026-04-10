namespace HFramework
{
	public enum RunMode
	{
		/**
		 * Legacy mode - uses the old Scenes / Definition files
		 */
		Legacy,

		/**
		 * Compatibility mode - uses both the new SexScript mode and the old Scenes / Definition files.
		 *
		 * This mode is used to try to maintain compatibility with existing mods, but may cause side effects if both
		 * does the same thing.
		 */
		Compatibility,

		/**
		 * Future mode - uses the new SexScript mode only.
		 */
		Future
	}
}
