using System.Diagnostics;

namespace YotanModCore
{
	internal class PLogger
	{
		public static ILogger _Logger = new UnityLogger();

		public static void SetLogger(ILogger logger)
		{
			_Logger = logger;
		}

		public static void LogInfo(object data)
		{
			_Logger.LogInfo(data);
		}

		public static void LogError(object data, bool includeStack = false)
		{
			_Logger.LogError(data);
			if (includeStack)
				_Logger.LogError(new StackTrace().ToString());
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
