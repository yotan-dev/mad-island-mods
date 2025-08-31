using YotanModCore.Console;
using YotanModCore.Items;

namespace YotanModCore
{
	public static class Initializer
	{
		private static bool initialized = false;

		public static void Init(ILogger logger)
		{
			if (initialized)
			{
				PLogger.LogError("YotanModCore: Already initialized");
				return;
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
		}
	}
}
