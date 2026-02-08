namespace HFramework
{
	internal class UnityLogger : ILogger
	{
		public void LogInfo(object data)
		{
			UnityEngine.Debug.Log(data);
		}

		public void LogError(object data)
		{
			UnityEngine.Debug.LogError(data);
		}

		public void LogWarning(object data)
		{
			UnityEngine.Debug.LogWarning(data);
		}

		public void LogDebug(object data)
		{
			UnityEngine.Debug.Log(data);
		}

		public void LogFatal(object data)
		{
			UnityEngine.Debug.LogError(data);
		}

		public void LogMessage(object data)
		{
			UnityEngine.Debug.Log(data);
		}
	}
}
