using BepInEx.Logging;

namespace Base
{
	internal class PLogger
	{
		// Not worth fighting with this error
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public static ManualLogSource _Logger;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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