using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFramework.Tree
{
	public class StartScriptNode : ActionNode
	{
		public string message;

		protected override void OnStart()
		{
			this.context.SexPlacePos = this.context.SexPlace.transform.Find("pos")?.position;
		}

		protected override void OnStop()
		{
		}

		protected override State OnUpdate()
		{
			foreach (var npc in this.context.Npcs)
			{
				npc.Common.nMove.actType = NPCMove.ActType.Wait;
				npc.Common.sex = CommonStates.SexState.Playing;
			}

			return State.Success;
		}
	}
}
