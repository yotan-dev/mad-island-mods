#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	/// <summary>
	/// H Framework provided context tags.
	/// Context Tags are used to give more information about the environment a script is running,
	/// helping events and other processing to make decisions.
	/// </summary>
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public static class ContextTags
	{
		// Interaction kinds
		public const string Normal = "Normal";

		public const string Forced = "Forced";

		// Locations
		public const string Daruma = "Daruma";

		public const string Toilet = "Toilet";

		static ContextTags() {
			LoadContextTags();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void LoadContextTags() {
			ContextTagRegistry.Register(Normal);
			ContextTagRegistry.Register(Forced);
			ContextTagRegistry.Register(Daruma);
			ContextTagRegistry.Register(Toilet);
		}
	}
}
