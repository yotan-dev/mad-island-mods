using YotanModCore.Console;
using YotanModCore.Events;
using YotanModCore.Items;

namespace YotanModCore
{
	public static class Initializer
	{
		private static bool initialized = false;

		/// <summary>
		/// Initializes YotanModCore. Should only be used by YotanModCoreLoader.
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		public static InitializerResult Init(ILogger logger)
		{
			if (initialized)
			{
				PLogger.LogError("YotanModCore: Already initialized");
				return null;
			}

			initialized = true;

			PLogger.SetLogger(logger);

			PLogger.LogInfo("Initializing YotanModCore...");
			PLogger.LogInfo($"> Game Version: {GameInfo.ToVersionString(GameInfo.GameVersion)}");
			PLogger.LogInfo($">> DLC: {GameInfo.HasDLC}");
			PLogger.LogInfo($">> Censor Type: {GameInfo.CensorType}");

			CommonUtils.Init();
			ConsoleManager.Instance.Init();

			CraftDB.Init();
			ItemDB.Init();

			PLogger.LogInfo("YotanModCore initialized!");

			return new InitializerResult()
			{
				Pre_GameManager_Start = GameLifecycleEvents.Pre_GamaManager_Start,
				Pre_SceneScript_SceneChange = GameLifecycleEvents.Pre_SceneScript_SceneChange,
			};
		}
	}
}
