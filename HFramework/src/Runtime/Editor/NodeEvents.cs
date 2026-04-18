using System;
using HFramework.ScriptNodes;

namespace HFramework.EditorUI.SexScripts
{
	public static class NodeEvents
	{
		public static event Action<ScriptNode> OnNodeChanged;

		public static event Action<ScriptNode> OnNodeIDChanged;

		public static void TriggerNodeChanged(ScriptNode node)
		{
			OnNodeChanged?.Invoke(node);
		}

		public static void TriggerNodeIDChanged(ScriptNode node)
		{
			OnNodeIDChanged?.Invoke(node);
		}
	}
}
