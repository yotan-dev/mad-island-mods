using UnityEngine;

namespace HFramework
{
	public static class Initializer
	{
		private static bool initialized = false;

		/// <summary>
		/// Initializes HFramework. Should only be used by HFrameworkLoader.
		/// </summary>
		/// <param name="logger"></param>
		public static void Init(ILogger logger)
		{
			if (initialized)
			{
				PLogger.LogError("HFramework: Already initialized");
				return;
			}

			initialized = true;

			PLogger.SetLogger(logger);

			PLogger.LogInfo("HFramework initialized!");
		}
	}
}
