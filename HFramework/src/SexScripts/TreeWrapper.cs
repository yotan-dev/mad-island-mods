using System.Collections;
using HFramework.Tree;

namespace HFramework.SexScripts
{
	public class TreeWrapper
	{
		public IEnumerator Run(BehaviourTree tree, CommonStates npcA, CommonStates npcB, SexPlace sexPlace)
		{
			BehaviourTree actualTree;
			try
			{
				PLogger.LogInfo("Tree is null? " + (tree == null));
				actualTree = tree.Clone();
				actualTree.context.NpcA = npcA;
				actualTree.context.NpcB = npcB;
				actualTree.context.SexPlace = sexPlace;
			}
			catch (System.Exception e)
			{
				PLogger.LogError($"Sex script start error: {e}");
				yield break;
			}


			// bool recoveryMode = false;
			while (actualTree.treeState == Node.State.Running)
			{
				try
				{
					actualTree.Update();
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
