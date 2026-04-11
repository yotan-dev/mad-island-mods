using System;
using System.Collections.Generic;
using System.Linq;
using HFramework.ScriptNodes;
using YotanModCore;

namespace HFramework.SexScripts.Info
{
	[Serializable]
	[Experimental]
	public class ConditionGroup
	{
		public List<Condition> Conditions;

		public bool Pass()
		{
			return this.Conditions.All(c => c.Pass());
		}

		public bool Pass(SexInfo info)
		{
			return this.Conditions.All(c => c.Pass(info));
		}
	}
}
