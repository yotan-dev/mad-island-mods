namespace HFramework
{
	using BepInEx.Logging;

	internal class PLogger
	{
		public static ManualLogSource _Logger;

		public static void LogInfo(object data)
		{
			_Logger.LogInfo(data);
		}

		public static void LogError(object data)
		{
			_Logger.LogError(data);
		}

		public static void LogWarning(object data)
		{
			_Logger.LogWarning(data);
		}

		public static void LogDebug(object data)
		{
			_Logger.LogDebug(data);
		}

		public static void LogFatal(object data)
		{
			_Logger.LogFatal(data);
		}

		public static void LogMessage(object data)
		{
			_Logger.LogMessage(data);
		}
	}
}
