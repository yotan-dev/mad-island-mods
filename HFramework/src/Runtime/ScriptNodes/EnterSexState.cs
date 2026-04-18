using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Enter Sex State")]
	public class EnterSexState : Action
	{
		public string message;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			foreach (var npc in this.context.Actors)
			{
				npc.Common.nMove.actType = NPCMove.ActType.Wait;
				npc.Common.sex = CommonStates.SexState.Playing;
			}

			return State.Success;
		}
	}
}
