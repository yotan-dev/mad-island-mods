namespace HFramework
{
	/// <summary>
	/// Configuration information passed to HFramework during initialization.
	/// This avoids namespace dependencies on HFrameworkLoader.
	/// </summary>
	public class ConfigInfo
	{
		public bool ReplaceOriginalScenes { get; set; } = true;
		public bool DebugConditions { get; set; } = false;

		public ConfigInfo() { }

		public ConfigInfo(bool replaceOriginalScenes, bool debugConditions)
		{
			ReplaceOriginalScenes = replaceOriginalScenes;
			DebugConditions = debugConditions;
		}
	}
}
