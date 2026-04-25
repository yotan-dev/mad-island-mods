using System.Collections;
using HFramework.ScriptNodes;
using UnityEngine;

namespace HFramework.SexScripts
{
	[Experimental]
	public class TreeWrapper
	{
		public IEnumerator Run(SexScript tree) {
			if (tree.Context != null && tree.Context.MenuSession == null) {
				var pos = tree.Context.ScriptPlace.GetCharacterPosition();
				tree.Context.MenuSession = new PropPanelMenuSession(tree.Context, pos);
			}

			// bool recoveryMode = false;
			while (tree.TreeState == ScriptNode.State.Running) {
				try {
					tree.Update();
				} catch (System.Exception e) {
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

			tree.Context?.MenuSession?.Close();
			if (tree.Context != null)
				tree.Context.MenuSession = null;

			PLogger.LogError($"Sex script finished");
		}
	}
}
