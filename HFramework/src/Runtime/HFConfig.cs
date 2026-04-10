namespace HFramework
{
	/// <summary>
	/// Singleton class for accessing configuration data in HFramework.
	/// This class is updated by HFrameworkLoader when needed.
	/// </summary>
	public class HFConfig
	{
		internal static HFConfig Instance = new HFConfig();

		public static HFConfig GetInstance() => Instance;

		/// <summary>
		/// Whether the mod should replace the H-Scenes original processing with its own.
		/// </summary>
		public bool ReplaceOriginalScenes { get; set; } = true;

		/// <summary>
		/// How the mod checks for scenes
		/// </summary>
		public RunMode RunMode { get; set; } = RunMode.Legacy;

		/// <summary>
		/// Is Legacy mode enabled (either in compatiility or actively chosen)
		/// </summary>
		public bool IsLegacyModeEnabled => this.RunMode == RunMode.Legacy;

		/// <summary>
		/// Is Modern mode enabled somehow (either in compatiility or actively chosen)
		/// </summary>
		public bool IsModernModeEnabled => this.RunMode != RunMode.Legacy;

		/// <summary>
		/// Whether to log debug information about conditions.
		/// </summary>
		public bool DebugConditions { get; set; } = false;
	}
}
