using System;

namespace HFramework.SexScripts.Info.Conditions
{
	[Serializable]
	[Experimental]
	public class Condition
	{
		public virtual bool CanStart()
		{
			PLogger.LogError($"{this.GetType().Name}: Condition.CanStart() not implemented!");
			return false;
		}

		public virtual bool CanExecute(SexInfo info)
		{
			PLogger.LogError($"{this.GetType().Name}: Condition.CanExecute() not implemented!");
			return false;
		}
	}
}
