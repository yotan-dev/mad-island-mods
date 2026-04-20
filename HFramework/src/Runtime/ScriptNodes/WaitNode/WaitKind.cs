using System;

namespace HFramework.ScriptNodes.WaitNode
{
	[Serializable]
	[Experimental]
	public abstract class WaitKind
	{
		public abstract void Start();

		public abstract bool IsDone();
	}
}
