using HFramework.Hook;
using HFramework.Scenes;
using YotanModCore.Events;

namespace HFramework
{
	public static class Initializer
	{
		private static bool initialized = false;

		/// <summary>
		/// Initializes HFramework. Should only be used by HFrameworkLoader.
		/// </summary>
		/// <param name="logger">Logger instance</param>
		/// <returns></returns>
		public static HFrameworkBridge Init(YotanModCore.ILogger logger)
		{
			if (initialized)
			{
				PLogger.LogError("HFramework: Already initialized");
				return null;
			}

			initialized = true;

			PLogger.SetLogger(logger);

			PLogger.LogInfo("Initializing HFramework...");
			PLogger.LogInfo($"> Replace Original Scenes: {HFConfig.Instance.ReplaceOriginalScenes}");
			PLogger.LogInfo($"> Debug Conditions: {HFConfig.Instance.DebugConditions}");

			if (HFConfig.Instance.ReplaceOriginalScenes)
			{
				GameLifecycleEvents.OnGameStartEvent += () => { SexMeter.Instance.Reload(); };
				HookManager.RegisterHooksEvent += CommonHooks.Instance.InitHooks;
			}

			PLogger.LogInfo("HFramework initialized!");

			return new HFrameworkBridge();
		}
	}
}
