using System;

namespace HFramework.SexScripts.Info.NpcConditions
{
	[Serializable]
	[Experimental]
	public class NpcCondition : INpcCondition
	{
		public virtual bool Pass(CommonStates common)
		{
			PLogger.LogError($"{this.GetType().Name}: Condition.CanStart() not implemented!");
			return false;
		}
	}
}
