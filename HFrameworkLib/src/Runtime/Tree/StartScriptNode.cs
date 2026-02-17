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
			// @TODO: Maybe move this to script creation
			if (this.context.SexPlace != null && this.context.SexPlacePos == null)
			{
				this.context.SexPlacePos = this.context.SexPlace.transform.Find("pos")?.position;
			}
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
