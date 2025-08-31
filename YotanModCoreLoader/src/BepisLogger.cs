using BepInEx.Logging;

namespace YotanModCore
{
	public class BepisLogger : ILogger
	{
		private ManualLogSource _Logger;

		public BepisLogger(ManualLogSource logger)
		{
			_Logger = logger;
		}

		public void LogInfo(object data)
		{
			_Logger.LogInfo(data);
		}

		public void LogError(object data)
		{
			_Logger.LogError(data);
		}

		public void LogWarning(object data)
		{
			_Logger.LogWarning(data);
		}

		public void LogDebug(object data)
		{
			_Logger.LogDebug(data);
		}

		public void LogFatal(object data)
		{
			_Logger.LogFatal(data);
		}

		public void LogMessage(object data)
		{
			_Logger.LogMessage(data);
		}
	}
}
