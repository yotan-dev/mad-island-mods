using UnityEngine;
using YotanModCore.Console;
using YotanModCore.Items;
using YotanModCore.NpcTalk;

namespace YotanModCore
{
	public static class Initializer
	{
		private static bool initialized = false;

		internal static AssetBundle Assets;

		/// <summary>
		/// Initializes YotanModCore. Should only be used by YotanModCoreLoader.
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		public static ModCoreBridge Init(ILogger logger)
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

			Assets = AssetBundle.LoadFromFile($"BepInEx/plugins/YotanModCore/YotanModCore.assets");

			CraftDB.Init();
			ItemDB.Init();

			NpcTalkManager.Init();

			PLogger.LogInfo("YotanModCore initialized!");

			return new ModCoreBridge();
		}
	}
}
