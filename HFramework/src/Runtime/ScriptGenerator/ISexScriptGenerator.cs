namespace HFramework.ScriptGenerator
{
	/// <summary>
	/// Interface for SexScript generators.
	/// </summary>
	public interface ISexScriptGenerator
	{
		/// <summary>
		/// Generates the SexScripts.
		/// It is the implementation's responsibility to save the changes with AssetDatabase.SaveAssets().
		/// </summary>
		void Generate();
	}
}
