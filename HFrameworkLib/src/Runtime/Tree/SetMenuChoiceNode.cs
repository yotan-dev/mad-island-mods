using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HFramework.Tree
{
	public class SetMenuChoiceNode : ActionNode
	{
		public string newChoiceId;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			this.context.PendingChoiceId = this.newChoiceId;
			return State.Success;
		}
	}
}
