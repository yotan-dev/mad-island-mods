namespace YotanModCore
{
	/// <summary>
	/// Interface to handle logs in YotanModCore.<br />
	/// This is meant as a workaround so we can give BepInEx logger from YotanModCoreLoader to YotanModCore.<br /><br />
	/// You can use this if you have a similar scenario, but you usually don't need it.
	/// </summary>
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
