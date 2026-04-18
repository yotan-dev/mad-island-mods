using System.IO;
using HFramework.SexScripts;
using UnityEditor;
using UnityEngine;

namespace HFramework.ScriptGenerator
{
	/// <summary>
	/// Base class to implement SexScript generators.
	/// Generators are used to programatically create SexScripts from templates,
	/// making it easier to bulk create scripts that has similar behavior.
	/// </summary>
	public abstract class BaseSexScriptGenerator : ISexScriptGenerator
	{
		/// <summary>
		/// Creates a new SexScript from a SexScript template (another SexScript asset).
		/// </summary>
		/// <param name="templatePath">Path to the template asset.</param>
		/// <param name="outPath">Path to the output asset.</param>
		/// <returns>The created SexScript.</returns>
		protected SexScript CreateFromTemplate(string templatePath, string outPath) {
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(outPath));

				AssetDatabase.CopyAsset(templatePath, outPath);
				AssetDatabase.SaveAssets();

				var script = AssetDatabase.LoadAssetAtPath<SexScript>(outPath);
				if (script == null) {
					throw new System.Exception($"Failed to load created script from \"{outPath}\"");
				}

				return script;
			} catch (System.Exception e) {
				Debug.LogError($"Failed to create from template at \"{templatePath}\" to \"{outPath}\": {e.Message}");
				throw e;
			}
		}

		/// <summary>
		/// Saves the current asset database, making sure your changes are persisted.
		/// It is usually a good idea to call it at the end of your Generate implementation.
		/// </summary>
		protected void Save() {
			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Generates the SexScripts.
		/// </summary>
		public abstract void Generate();
	}
}
