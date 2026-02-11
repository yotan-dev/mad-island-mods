using System.Collections;
using HFramework.Tree;

namespace HFramework.SexScripts
{
	public class TreeWrapper
	{
		public IEnumerator Run(BehaviourTree tree)
		{
			// bool recoveryMode = false;
			while (tree.treeState == Node.State.Running)
			{
				try
				{
					tree.Update();
				}
				catch (System.Exception e)
				{
					// if (recoveryMode) {
					// 	PLogger.LogError($"Sex script update error: {e} -- Already in recovery mode, giving up");
					// 	break;
					// }

					PLogger.LogError($"Sex script update error: {e} -- Trying to finish it");
					break; // @TODO: implement a recovery mode

					// try {
					// 	tree.ChangeState(null);
					// }
					// catch (System.Exception e2) {
					// 	PLogger.LogError($"Sex script finish error: {e2}");
					// }
				}

				yield return null;
			}

			PLogger.LogError($"Sex script finished");
		}
	}
}
