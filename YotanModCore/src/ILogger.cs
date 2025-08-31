namespace YotanModCore
{
	public interface ILogger
	{
		void LogInfo(object data);
		void LogError(object data);
		void LogWarning(object data);
		void LogDebug(object data);
		void LogFatal(object data);
		void LogMessage(object data);
	}
}
