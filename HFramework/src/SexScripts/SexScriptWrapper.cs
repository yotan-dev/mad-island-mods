using System.Collections;

namespace HFramework.SexScripts
{
	public class SexScriptWrapper
	{
		public IEnumerator Run(CommonSexNpcScript script)
		{
			try
			{
				script.Start();
			}
			catch (System.Exception e)
			{
				PLogger.LogError($"Sex script start error: {e}");
			}


			bool recoveryMode = false;
			while (!script.IsCompleted)
			{
				try
				{
					script.Update();
				}
				catch (System.Exception e)
				{
					if (recoveryMode) {
						PLogger.LogError($"Sex script update error: {e} -- Already in recovery mode, giving up");
						break;
					}

					PLogger.LogError($"Sex script update error: {e} -- Trying to finish it");

					try {
						script.ChangeState(null);
					}
					catch (System.Exception e2) {
						PLogger.LogError($"Sex script finish error: {e2}");
					}
				}

				yield return null;
			}
		}
	}
}
