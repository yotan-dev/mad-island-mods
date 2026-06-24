using System;
using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Flow.WaitNode
{
	[Serializable]
	[Experimental]
	[MovedFrom(true, "HFramework.ScriptNodes.WaitNodes", null, "WaitKind")]
	public abstract class WaitKind
	{
		public abstract void Start();

		public abstract bool IsDone();
	}
}
