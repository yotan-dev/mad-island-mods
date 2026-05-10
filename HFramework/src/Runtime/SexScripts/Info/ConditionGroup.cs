using System;
using System.Linq;
using UnityEngine;
using HFramework.SexScripts.Info.Conditions;

namespace HFramework.SexScripts.Info
{
	[Serializable]
	[Experimental]
	public class ConditionGroup
	{
		[SerializeReference]
		[Subclass]
		public Condition[] Conditions = new Condition[0];

		public ConditionGroup() { }

		public ConditionGroup(params Condition[] conditions) {
			this.Conditions = conditions;
		}

		public bool CanStart() {
			return this.Conditions.All(c => c.CanStart());
		}

		public bool CanExecute(SexInfo info) {
			return this.Conditions.All(c => c.CanExecute(info));
		}
	}
}
