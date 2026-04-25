using HFramework.Events.EventHandlers;
using HFramework.Hook;
using HFramework.Scenes;
using YotanModCore.Events;

namespace HFramework
{
	public static class Initializer
	{
		private static bool initialized = false;

		/// <summary>
		/// Path to the HFramework folder, to be used when needing to refer to mod files
		/// </summary>
		internal static string HFrameworkFolderPath { get; private set; }

		/// <summary>
		/// Initializes HFramework. Should only be used by HFrameworkLoader.
		/// </summary>
		/// <param name="logger">Logger instance</param>
		/// <param name="hframeworkLoaderDllPath">Path to the plugin DLL</param>
		/// <returns></returns>
		public static HFrameworkBridge Init(YotanModCore.ILogger logger, string hframeworkLoaderDllPath) {
			if (initialized) {
				PLogger.LogError("HFramework: Already initialized");
				return null;
			}

			initialized = true;

			PLogger.SetLogger(logger);

			HFrameworkFolderPath = System.IO.Path.GetDirectoryName(hframeworkLoaderDllPath);

			PLogger.LogInfo("Initializing HFramework...");
			PLogger.LogInfo($"> Replace Original Scenes: {HFConfig.Instance.ReplaceOriginalScenes}");
			PLogger.LogInfo($"> Debug Conditions: {HFConfig.Instance.DebugConditions}");

			if (HFConfig.Instance.ReplaceOriginalScenes) {
				GameLifecycleEvents.OnGameStartEvent += () => { SexMeter.Instance.Reload(); };
				HookManager.RegisterHooksEvent += CommonHooks.Instance.InitHooks;
			}

			if (HFConfig.Instance.IsModernModeEnabled) {
				DefaultSexEventHandler.Instance.Init();
				RapeCountEventHandler.Instance.Init();
			}

			PLogger.LogInfo("HFramework initialized!");

			return new HFrameworkBridge();
		}
	}
}
