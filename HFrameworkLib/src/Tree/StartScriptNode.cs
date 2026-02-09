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
			NPCMove aMove = this.context.NpcA.GetComponent<NPCMove>();
			NPCMove bMove = this.context.NpcB.GetComponent<NPCMove>();
			aMove.actType = NPCMove.ActType.Wait;
			bMove.actType = NPCMove.ActType.Wait;
			this.context.NpcA.sex = CommonStates.SexState.Playing;
			this.context.NpcB.sex = CommonStates.SexState.Playing;

			return State.Success;
		}
	}
}
