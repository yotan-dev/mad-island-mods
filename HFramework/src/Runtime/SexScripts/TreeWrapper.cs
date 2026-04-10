using System.Collections;
using HFramework.Tree;
using UnityEngine;

namespace HFramework.SexScripts
{
	public class TreeWrapper
	{
		public IEnumerator Run(BehaviourTree tree)
		{
			if (tree.context != null && tree.context.MenuSession == null)
			{
				var pos = tree.context.SexPlacePos ?? tree.context.SexPlace?.transform.position ?? Vector3.zero;
				tree.context.MenuSession = new PropPanelMenuSession(tree.context, pos);
			}

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

			tree.context?.MenuSession?.Close();
			if (tree.context != null)
				tree.context.MenuSession = null;

			PLogger.LogError($"Sex script finished");
		}
	}
}
